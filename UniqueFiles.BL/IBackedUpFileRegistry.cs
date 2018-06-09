using System.IO;

namespace UniqueFiles.BL
{
    public interface IBackedUpFileRegistry
    {
        void Add(FileInfo fileInfo);
    }
}