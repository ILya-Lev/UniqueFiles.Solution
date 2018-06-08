using System.Collections.Generic;

namespace UniqueFiles.BL
{
    public interface IFileSystemEntityProvider
    {
        IEnumerable<string> GetNames(string folder);
    }
}