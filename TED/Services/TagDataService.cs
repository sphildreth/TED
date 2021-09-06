using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Roadie.Library;
using Roadie.Library.Caching;
using Roadie.Library.Configuration;
using Roadie.Library.Imaging;
using Roadie.Library.MetaData.Audio;
using Roadie.Library.MetaData.ID3Tags;
using Roadie.Library.Processors;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.Json;
using System.Threading.Tasks;
using TED.Models;
using TED.Models.TagData;

namespace TED.Services
{
    public sealed class TagDataService
    {
        private IOptions<TEDSettings> TEDSettings { get; }

        private IRoadieSettings RoadieSettings { get; }

        private ILogger Logger { get; }

        private IEventMessageLogger MessageLogger { get; }

        private ID3TagsHelper TagsHelper { get; }

        private ICacheManager CacheManager { get; }

        public TagDataService(
            IOptions<TEDSettings> tedSettings,
            IRoadieSettings roadieSettings,
            ILogger<TagDataService> logger,
            ICacheManager cachemanager)
        {
            TEDSettings = tedSettings;
            RoadieSettings = roadieSettings;
            Logger = logger;

            MessageLogger = new EventMessageLogger<TagDataService>();
            MessageLogger.Messages += MessageLogger_Messages;

            CacheManager = cachemanager;

            var tagHelperLooper = new EventMessageLogger<ID3TagsHelper>();
            tagHelperLooper.Messages += MessageLogger_Messages;
            TagsHelper = new ID3TagsHelper(RoadieSettings, CacheManager, tagHelperLooper);
        }

        public async Task<bool> CreateTagDataForFileDirectoryAsync(FileDirectory fileDirectory)
        {
            if(string.IsNullOrEmpty(fileDirectory?.FullPath))
            {
                return false;
            }
            var fileDirectoryInfo = new DirectoryInfo(fileDirectory.FullPath);
            if(!fileDirectoryInfo.Exists)
            {
                return false;
            }
            var result = new TagData();

            try
            {
                RunScript("PreDiscover.ps1", fileDirectory.FullPath);
                SfvFile sfvFile = null;
                var sfvFiles = Directory.GetFiles(fileDirectory.FullPath, "*.sfv", SearchOption.AllDirectories);
                if (sfvFiles.Any())
                {
                    sfvFile = await ParseSfvFile(sfvFiles.First()).ConfigureAwait(false);
                }
                var mediaFiles = Directory.GetFiles(fileDirectory.FullPath, "*.mp3", SearchOption.AllDirectories);
                if (mediaFiles?.Any() == true)
                {
                    var sw = Stopwatch.StartNew();
                    var resultMedias = new List<Media>();
                    var tagDataForFileDirectory = new List<AudioMetaData>();
                    foreach (var mediaFile in mediaFiles)
                    {
                        var fileInfo = new FileInfo(mediaFile);
                        var tagData = TagsHelper.MetaDataForFile(fileInfo.FullName, true);
                        if (!tagData?.IsSuccess ?? false)
                        {
                            Trace.WriteLine($"INVALID: Missing: {ID3TagsHelper.DetermineMissingRequiredMetaData(tagData.Data)}");
                            continue;
                        }
                        tagDataForFileDirectory.Add(tagData.Data);
                    }
                    var resultReleases = new List<Release>();
                    foreach (var tabLibReleaseGroups in tagDataForFileDirectory.GroupBy(x => x.Release))
                    {
                        var tabLibsForRelease = tabLibReleaseGroups.ToArray();
                        var releaseImages = ImageHelper.FindImageTypeInDirectory(tabLibsForRelease.First().FileInfo.Directory, Roadie.Library.Enums.ImageType.Release, SearchOption.TopDirectoryOnly).ToList();
                        releaseImages.AddRange(ImageHelper.FindImageTypeInDirectory(tabLibsForRelease.First().FileInfo.Directory, Roadie.Library.Enums.ImageType.ReleaseSecondary, SearchOption.TopDirectoryOnly));
                        resultReleases.Add(new Release
                        {
                            Artist = tabLibsForRelease.First().Artist,
                            Directory = fileDirectory.FullPath,
                            ModifiedDate = fileDirectoryInfo.LastWriteTimeUtc,
                            Name = tabLibsForRelease.First().Release,
                            Year = tabLibsForRelease.First().Year.Value,
                            HasTagImage = tabLibsForRelease.Any(x => x.Images?.Any() ?? false),
                            ImageFiles = releaseImages.Select(x => x.FullName).Distinct().ToArray(),
                            ReleaseMedia = tabLibsForRelease.GroupBy(x => x.Disc ?? 1).Select(x => new ReleaseMedia
                            {
                                ReleaseMediaNumber = x.Key,
                                Media = x.Select(xx => new Media
                                {
                                    FileName = xx.FileInfo.Name,
                                    FileSize = xx.FileInfo.Length,
                                    Length = xx.TotalSeconds,
                                    Title = xx.Title,
                                    TrackNumber = xx.TrackNumber.Value
                                })
                            })
                        });
                    }
                    if (resultReleases.Count() == 1)
                    {
                        var releaseImages = ImageHelper.FindImageTypeInDirectory(fileDirectoryInfo, Roadie.Library.Enums.ImageType.Release, SearchOption.TopDirectoryOnly).ToList();
                        releaseImages.AddRange(ImageHelper.FindImageTypeInDirectory(fileDirectoryInfo, Roadie.Library.Enums.ImageType.ReleaseSecondary, SearchOption.TopDirectoryOnly));
                        releaseImages.AddRange(resultReleases.First().ImageFiles.Select(x => new FileInfo(x)));
                        resultReleases.First().ImageFiles = releaseImages.Select(x => x.FullName).Distinct().OrderBy(x => x).ToArray();
                        resultReleases.First().ExpectedTrackNumber = sfvFile == null ? 0 : sfvFile.Entries.Count(x => x.TrackNumber > 0);
                    }
                    result.Releases = resultReleases;
                    result.CreatedDate = DateTime.UtcNow;
                    var artistImages = ImageHelper.FindImageTypeInDirectory(fileDirectoryInfo, Roadie.Library.Enums.ImageType.Artist, SearchOption.TopDirectoryOnly).ToList();
                    artistImages.AddRange(ImageHelper.FindImageTypeInDirectory(fileDirectoryInfo, Roadie.Library.Enums.ImageType.ArtistSecondary, SearchOption.TopDirectoryOnly));
                    result.ArtistImageFiles = artistImages.Select(x => x.FullName).Distinct().ToArray();
                    sw.Stop();
                    result.GenerationDuration = sw.ElapsedMilliseconds;
                    string jsonString = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(Path.Combine(fileDirectory.FullPath, "tagData.json"), jsonString).ConfigureAwait(false);
                    RunScript("PostDiscover.ps1", fileDirectory.FullPath);
                    return true;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }            
            return false;
        }

        private string RunScript(string scriptFilename, string directoryToInspect)
        {
            if (string.IsNullOrEmpty(scriptFilename) || string.IsNullOrEmpty(directoryToInspect))
            {
                return null;
            }

            try
            {
                var scriptFolder = TEDSettings.Value.ScriptingFolder;
                if(string.IsNullOrEmpty(scriptFolder))
                {
                    Trace.WriteLine($"RunScript: Invalid ScriptingFolder");
                    return null;
                }
                scriptFilename = Path.Combine(scriptFolder, scriptFilename);
                if (!File.Exists(scriptFilename))
                {
                    Trace.WriteLine($"RunScript: Script Not Found: [{ scriptFilename }]");
                    return null;
                }
                Console.WriteLine($"Running Script: [{ scriptFilename }]");
                var script = File.ReadAllText(scriptFilename);
                using (var ps = PowerShell.Create())
                {
                    var r = string.Empty;
                    var results = ps.AddScript(script)
                                    .AddParameter("DirectoryToInspect", directoryToInspect)
                                    .Invoke();
                    foreach (var result in results)
                    {
                        r += result + Environment.NewLine;
                    }
                    Trace.Write($"RunScript: [{ r }]");
                    return r;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"📛 Error with Script File [{scriptFilename}], Error [{ex}] ");
            }
            return null;
        }

        private void MessageLogger_Messages(object sender, EventMessage e)
        {
            Console.WriteLine($"Log Level [{e.Level}] Log Message [{e.Message}] ");
            var message = e.Message;
            switch (e.Level)
            {
                case LogLevel.Trace:
                    Logger.LogTrace(message);
                    break;

                case LogLevel.Debug:
                    Logger.LogDebug(message);
                    break;

                case LogLevel.Information:
                    Logger.LogInformation(message);
                    break;

                case LogLevel.Warning:
                    Logger.LogWarning(message);
                    break;

                case LogLevel.Critical:
                    Logger.LogCritical(message);
                    break;
            }
        }

        public static async Task<SfvFile> ParseSfvFile(string sfvFileName)
        {
            var result = new SfvFile
            {
                Name = sfvFileName
            };
            var data = new List<SfvFileEntry>();
            foreach(var line in (await File.ReadAllLinesAsync(sfvFileName).ConfigureAwait(false)).Where(x => !string.IsNullOrEmpty(x) && !x.StartsWith(";")))
            {
                var parts = line.Split(' ');
                data.Add(new SfvFileEntry
                {
                    FileName = string.Join(' ', parts.Take(parts.Length -1)),
                    Crc32 = parts[parts.Length - 1]
                });
            };
            result.Entries = data;
            return result;
        }
    }
}
