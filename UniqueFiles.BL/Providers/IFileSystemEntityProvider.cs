using System.Collections.Generic;

namespace UniqueFiles.BL.Providers
{
    public interface IFileSystemEntityProvider
    {
        IEnumerable<string> GetDescendantPaths(string folder);
    }
}