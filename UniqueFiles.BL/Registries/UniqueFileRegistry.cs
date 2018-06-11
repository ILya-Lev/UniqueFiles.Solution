using System;
using System.Collections.Generic;
using System.IO;

namespace UniqueFiles.BL.Registries
{
    public class UniqueFilesRegistry : IUniqueFilesRegistry
    {
        private readonly HashSet<string> _registry = new HashSet<string>();

        public bool Contains(FileInfo fileInfo)
        {
            return _registry.Contains(fileInfo?.Name ?? "");
        }

        public void Add(FileInfo fileInfo)
        {
            if (string.IsNullOrWhiteSpace(fileInfo?.Name))
                throw new InvalidOperationException("Cannot register empty file name as a unique one.");

            if (!_registry.Add(fileInfo.Name))
                throw new InvalidOperationException($"'{fileInfo.Name}' file is already registered as unique" +
                                                    " and should not be passed here!");
        }
    }
}