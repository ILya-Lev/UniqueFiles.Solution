using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniqueFiles.BL
{
    public class DirectoryProvider : IFileSystemEntityProvider
    {
        private readonly string _backupRoot;

        public DirectoryProvider(string backupRoot)
        {
            _backupRoot = backupRoot;
        }

        public IEnumerable<string> GetDescendantPaths(string folder)
        {
            if (!Directory.Exists(folder))
                return Enumerable.Empty<string>();

            return Directory.GetDirectories(folder)
                .Where(path => !string.Equals(path, _backupRoot, StringComparison.OrdinalIgnoreCase));
        }
    }
}