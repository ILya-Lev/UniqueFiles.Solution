using System.Collections.Generic;
using System.IO;
using UniqueFiles.BL.Interfaces;

namespace UniqueFiles.BL.Registries
{
    public class UniqueFileRegistry : IUniqueFileRegistry
    {
        private readonly Dictionary<string, FileInfo> _registry = new Dictionary<string, FileInfo>();

        public bool Contains(FileInfo fileInfo)
        {
            return _registry.ContainsKey(fileInfo.Name);
        }

        public void Add(FileInfo fileInfo)
        {
            _registry.Add(fileInfo.Name, fileInfo);
        }
    }
}