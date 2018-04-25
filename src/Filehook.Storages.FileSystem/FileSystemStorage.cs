using Filehook.Abstractions;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
            return _locationTemplateParser.SetBase(relativeLocation, _options.CdnUrl);
        }

        // TODO tests
        public Task<bool> ExistsAsync(string relativeLocation)
        {
            var location = _locationTemplateParser.SetBase(relativeLocation, _options.BasePath);

            return Task.FromResult(File.Exists(location));
        }

        public async Task<string> SaveAsync(string relativeLocation, Stream stream)
        {
            var location = _locationTemplateParser.SetBase(relativeLocation, _options.BasePath);

            var directoryPath = Path.GetDirectoryName(location);

            if (!Directory.Exists(directoryPath))
            {
                _logger.LogDebug("Created empty directory '{0}'", directoryPath);

                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(location, FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fileStream);
            }

            _logger.LogInformation("Saved file on location '{0}'", location);

            return location;
        }

        // TODO tests
        public Task<bool> RemoveAsync(string relativeLocation)
        {
            var location = _locationTemplateParser.SetBase(relativeLocation, _options.BasePath);

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
