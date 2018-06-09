using System.Collections.Generic;
using System.IO;

namespace UniqueFiles.BL
{
    public class UniqueFileRegistry : IUniqueFileRegistry
    {
        private readonly Dictionary<string, FileInfo> _registry = new Dictionary<string, FileInfo>();
        //public UniqueFileRegistry(IFileKeyGenerator fileKeyGenerator)
        //{
        //}

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