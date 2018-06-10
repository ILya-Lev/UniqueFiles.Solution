using System.IO;

namespace UniqueFiles.BL.Registries
{
    public interface IUniqueFileRegistry
    {
        bool Contains(FileInfo fileInfo);
        void Add(FileInfo fileInfo);
    }
}