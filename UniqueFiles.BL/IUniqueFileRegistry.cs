using System.IO;

namespace UniqueFiles.BL
{
    public interface IUniqueFileRegistry
    {
        bool Contains(FileInfo fileInfo);
        void Add(FileInfo fileInfo);
    }
}