using System.IO;

namespace UniqueFiles.BL.Interfaces
{
    public interface IBackedUpFileRegistry
    {
        void Add(FileInfo fileInfo);
    }
}