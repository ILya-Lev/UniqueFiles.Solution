using System.IO;

namespace UniqueFiles.BL.Interfaces
{
    public interface IUniqueFileRegistry
    {
        bool Contains(FileInfo fileInfo);
        void Add(FileInfo fileInfo);
    }
}