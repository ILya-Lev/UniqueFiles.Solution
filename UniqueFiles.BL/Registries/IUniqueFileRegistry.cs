using System.IO;

namespace UniqueFiles.BL.Registries
{
    public interface IUniqueFilesRegistry
    {
        bool Contains(FileInfo fileInfo);
        void Add(FileInfo fileInfo);
    }
}