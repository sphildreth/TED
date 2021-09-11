using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TED.Models;
using TED.Models.TagData;

namespace TED.Services
{
    public sealed class FileDirectoryService
    {
        public IEnumerable<FileDirectory> GetFileDirectories(string root = null, SearchOption? searchOption = SearchOption.TopDirectoryOnly)
        {
            var result = new List<FileDirectory>();
            root ??= @"G:\complete"; // TODO get from config
            foreach (var d in Directory.GetDirectories(root, "*.*", searchOption ?? SearchOption.TopDirectoryOnly))
            {
                var di = new DirectoryInfo(d);
                var td = TagDataFoundForFileDirectory(di);
                var fd = new FileDirectory
                {
                    Id = Guid.NewGuid(),
                    Name = di.Name,
                    FullPath = Path.Combine(root, di.Name),
                    NumberOfMediaFound = NumberOfMediaFilesForFileDirectory(di),
                    HasTagData = td.Item1,
                    IsTagDataValid = td.Item2?.IsValid ?? false,
                };
                fd.IsSelected = !fd.HasTagData || !fd.IsTagDataValid;
                result.Add(fd);
            }
            return result;
        }

        private (bool, TagData) TagDataFoundForFileDirectory(DirectoryInfo directory)
        {
            var tagDataFilename = directory.GetFiles("tagData.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if(tagDataFilename != null)
            {
                return (true, JsonSerializer.Deserialize<TagData>(File.ReadAllText(tagDataFilename.FullName)));
            }
            return (false, null);
        }

        private long NumberOfMediaFilesForFileDirectory(DirectoryInfo directory)
        {
            return directory.GetFiles("*.mp3", SearchOption.AllDirectories).LongLength;
        }
    }
}