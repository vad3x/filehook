using System;
using System.IO;
using System.Security.Cryptography;
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

        private readonly ILocationTemplateParser _locationTemplateParser;

        private readonly ILogger _logger;

        public FileSystemStorage(
            IOptions<FileSystemStorageOptions> options,
            ILocationTemplateParser locationTemplateParser,
            ILogger<FileSystemStorage> logger)
        {
            _options = options.Value;
            _locationTemplateParser = locationTemplateParser;
            _logger = logger;
        }

        public string Name => _options.Name;

        // TODO tests
        public string GetUrl(string relativeLocation)
        {
            return _locationTemplateParser.SetRoot(relativeLocation, _options.HostUrl);
        }

        // TODO tests
        public Task<bool> ExistsAsync(string relativeLocation)
        {
            var location = _locationTemplateParser.SetRoot(relativeLocation, _options.Root);

            return Task.FromResult(File.Exists(location));
        }

        public async Task<FileStorageSavingResult> SaveAsync(string key, FilehookFileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            Stream stream = fileInfo.FileStream;

            var checksum = GetMD5Checksum(stream);
            var byteSize = stream.Length;

            var newFileName = _options.FileName(key, checksum, fileInfo);

            var location = _options.Location(_options.Root, null, null, null, key, newFileName);

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

            return FileStorageSavingResult.Success(newFileName, location, checksum, byteSize);
        }

        public Task<bool> RemoveFileAsync(string fileName)
        {
            var relativeLocation = _locationTemplateParser.Parse(
                filename: fileName,
                // TODO options
                locationTemplate: ":base/public/blobs/:filename");

            var location = _locationTemplateParser.SetRoot(relativeLocation, _options.Root);

            if (File.Exists(location))
            {
                File.Delete(location);

                _logger.LogInformation("Removed file from location '{0}'", location);

                return Task.FromResult(true);
            }

            _logger.LogDebug("File on location '{0}' is not found", location);

            return Task.FromResult(false);
        }

        // TODO tests
        public Task<bool> RemoveAsync(string relativeLocation)
        {
            var location = _locationTemplateParser.SetRoot(relativeLocation, _options.Root);

            if (File.Exists(location))
            {
                File.Delete(location);

                _logger.LogInformation("Removed file from location '{0}'", location);

                return Task.FromResult(true);
            }

            _logger.LogDebug("File on location '{0}' is not found", location);

            return Task.FromResult(false);
        }

        private static string GetMD5Checksum(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                stream.Position = 0;

                var hash = md5.ComputeHash(stream);

                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public Task<string> SaveAsync(string relativeLocation, Stream stream, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
