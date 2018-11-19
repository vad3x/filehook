using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Filehook.Abstractions
{
    public interface IFileStorage
    {
        string Name { get; }

        string GetUrl(string relativeLocation);

        Task<bool> ExistsAsync(string relativeLocation);

        Task<string> SaveAsync(string relativeLocation, Stream stream, CancellationToken cancellationToken = default);

        Task<FileStorageSavingResult> SaveAsync(string key, FilehookFileInfo fileInfo, CancellationToken cancellationToken = default);

        Task<bool> RemoveAsync(string relativeLocation);

        Task<bool> RemoveFileAsync(string fileName);
    }
}
