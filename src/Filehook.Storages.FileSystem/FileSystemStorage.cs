using Filehook.Abstractions;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace Filehook.Storages.FileSystem
{
    public class FileSystemStorage : IFileStorage
    {
        private readonly FileSystemStorageOptions _options;

        private readonly ILocationTemplateParser _locationTemplateParser;

        public FileSystemStorage(
            IOptions<FileSystemStorageOptions> options,
            ILocationTemplateParser locationTemplateParser)
        {
            _options = options.Value;
            _locationTemplateParser = locationTemplateParser;
        }

        public string Name { get { return FileSystemConsts.FileSystemStorageName; } }

        // TODO tests
        public string GetUrl(string relativeLocation)
        {
            return _locationTemplateParser.SetBase(relativeLocation, _options.CdnUrl);
        }

        // TODO tests
        public Task<bool> ExistsAsync(string relativeLocation)
        {
            var location = _locationTemplateParser.SetBase(relativeLocation, _options.CdnUrl);

            return Task.FromResult(File.Exists(location));
        }

        public async Task<string> SaveAsync(string relativeLocation, Stream stream)
        {
            var location = _locationTemplateParser.SetBase(relativeLocation, _options.BasePath);

            var directoryPath = Path.GetDirectoryName(location);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(location, FileMode.OpenOrCreate, FileAccess.Write))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fileStream);
            }

            return location;
        }
    }
}
