using System.IO;

namespace UniqueFiles.BL.Registries
{
    public interface IBackedUpFilesRegistry
    {
        void Add(FileInfo fileInfo);
    }
}