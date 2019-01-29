using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Filehook.Abstractions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Filehook.Storages.FileSystem
{
    public class FileSystemStorage : IFileStorage
    {
        private readonly FileSystemStorageOptions _options;

        private readonly ILogger _logger;

        public FileSystemStorage(
            IOptions<FileSystemStorageOptions> options,
            ILogger<FileSystemStorage> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        // TODO tests
        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            var location = _options.RelativeLocation(_options.Root, key);

            return Task.FromResult(File.Exists(location));
        }

        public async Task<FileStorageSavingResult> SaveAsync(string key, FilehookFileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            Stream stream = fileInfo.FileStream;

            var checksum = stream.GetMD5Checksum();
            var byteSize = stream.GetByteSize();

            var location = _options.RelativeLocation(_options.Root, key);

            var directoryPath = Path.GetDirectoryName(location);

            if (!Directory.Exists(directoryPath))
            {
                _logger.LogDebug("Created empty directory '{0}'", directoryPath);

                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(location, FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fileStream, 81920, cancellationToken)
                    .ConfigureAwait(false);
            }

            _logger.LogInformation("Saved file on location '{0}'", location);

            return FileStorageSavingResult.Success(location, checksum, byteSize);
        }

        public Task<bool> RemoveFileAsync(string key, CancellationToken cancellationToken = default)
        {
            var location = _options.RelativeLocation(_options.Root, key);

            if (File.Exists(location))
            {
                File.Delete(location);

                _logger.LogInformation("Removed file from location '{0}'", location);

                return Task.FromResult(true);
            }

            _logger.LogDebug("File on location '{0}' is not found", location);

            return Task.FromResult(false);
        }
    }
}
