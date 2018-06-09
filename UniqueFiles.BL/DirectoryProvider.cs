using System.Collections.Generic;
using System.IO;

namespace UniqueFiles.BL
{
    public class DirectoryProvider : IFileSystemEntityProvider
    {
        public IEnumerable<string> GetFullPath(string folder)
        {
            return Directory.GetDirectories(folder);
        }
    }
}