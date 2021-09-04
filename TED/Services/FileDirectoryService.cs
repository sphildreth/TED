using System;
using System.Collections.Generic;
using System.IO;
using TED.Models;

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
                result.Add(new FileDirectory
                {
                    Id = Guid.NewGuid(),
                    Name = di.Name,
                    FullPath = Path.Combine(root, di.Name),
                    NumberOfMediaFound = NumberOfMediaFilesForFileDirectory(di),
                    HasTagData = TagDataFoundForFileDirectory(di),
                });
            }
            return result;
        }

        private bool TagDataFoundForFileDirectory(DirectoryInfo directory)
        {
            return directory.GetFiles("tagData.json", SearchOption.TopDirectoryOnly).Length > 0;
        }

        private long NumberOfMediaFilesForFileDirectory(DirectoryInfo directory)
        {
            return directory.GetFiles("*.mp3", SearchOption.AllDirectories).LongLength;
        }
    }
}