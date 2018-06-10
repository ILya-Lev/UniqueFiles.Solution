using System.Collections.Generic;
using System.IO;
using UniqueFiles.BL.Interfaces;

namespace UniqueFiles.BL.Registries
{
    public class UniqueFileRegistry : IUniqueFileRegistry
    {
        private readonly HashSet<string> _registry = new HashSet<string>();

        public bool Contains(FileInfo fileInfo)
        {
            return _registry.Contains(fileInfo.Name);
        }

        public void Add(FileInfo fileInfo)
        {
            _registry.Add(fileInfo.Name);
        }
    }
}