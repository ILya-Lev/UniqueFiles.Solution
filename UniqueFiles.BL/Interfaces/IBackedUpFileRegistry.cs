using System.IO;

namespace UniqueFiles.BL.Interfaces
{
    public interface IBackedUpFilesRegistry
    {
        void Add(FileInfo fileInfo);
    }
}