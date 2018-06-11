using System.IO;
using System.Threading.Tasks;

namespace UniqueFiles.BL.Registries
{
    public interface IBackedUpFilesRegistry
    {
        Task AddAsync(FileInfo fileInfo);
    }
}