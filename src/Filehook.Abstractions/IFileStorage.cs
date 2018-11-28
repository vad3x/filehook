using System.Threading;
using System.Threading.Tasks;

namespace Filehook.Abstractions
{
    public interface IFileStorage
    {
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

        Task<FileStorageSavingResult> SaveAsync(string key, FilehookFileInfo fileInfo, CancellationToken cancellationToken = default);

        Task<bool> RemoveFileAsync(string key, CancellationToken cancellationToken = default);
    }
}
