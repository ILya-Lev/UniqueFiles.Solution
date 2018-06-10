using System.Collections.Generic;

namespace UniqueFiles.BL.Interfaces
{
    public interface IFileSystemEntityProvider
    {
        IEnumerable<string> GetDescendantPaths(string folder);
    }
}