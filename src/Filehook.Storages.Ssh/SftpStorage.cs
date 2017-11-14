using System;
using System.IO;
using System.Threading.Tasks;
using Filehook.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Async;
using Renci.SshNet.Common;

namespace Filehook.Storages.Ssh
{
    public class SftpStorage : IFileStorage
    {
        private readonly SftpStorageOptions _options;

        private readonly ILocationTemplateParser _locationTemplateParser;

        private readonly ILogger<SftpStorage> _logger;

        public SftpStorage(
            IOptions<SftpStorageOptions> options,
            ILocationTemplateParser locationTemplateParser,
            ILogger<SftpStorage> logger)
        {
            _options = options.Value;
            _locationTemplateParser = locationTemplateParser;
            _logger = logger;
        }

        public string Name => _options.Name;

        public Task<bool> ExistsAsync(string relativeLocation)
        {
            var fullPath = _locationTemplateParser.SetBase(relativeLocation, _options.BasePath);

            using (var client = new SftpClient(_options.HostName, _options.Port, _options.UserName, _options.Password))
            {
                client.Connect();

                return Task.FromResult(client.Exists(fullPath));
            }
        }

        public string GetUrl(string relativeLocation)
        {
            return ToAbsoluteUrl(relativeLocation);
        }

        public Task<bool> RemoveAsync(string relativeLocation)
        {
            using (var client = new SftpClient(_options.HostName, _options.Port, _options.UserName, _options.Password))
            {
                client.Connect();

                try
                {
                    client.DeleteFile(relativeLocation);
                }
                catch (SftpPathNotFoundException)
                {
                    _logger.LogError("File with path '{path}' could not be found", relativeLocation);

                    return Task.FromResult(false);
                }

                return Task.FromResult(true);
            }
        }

        public async Task<string> SaveAsync(string relativeLocation, Stream stream)
        {
            var fullPath = _locationTemplateParser.SetBase(relativeLocation, _options.BasePath);

            using (var client = new SftpClient(_options.HostName, _options.Port, _options.UserName, _options.Password))
            {
                client.Connect();

                var fileName = Path.GetFileName(fullPath);
                var directoryPath = fullPath.Remove(fullPath.Length - fileName.Length, fileName.Length);

                CreateDirectoryRecursively(client, directoryPath);

                stream.Position = 0;
                await client.UploadAsync(stream, fullPath, true);
            }

            var location = ToAbsoluteUrl(relativeLocation);

            _logger.LogInformation("Created '{location}'", location);

            return location;
        }

        private string ToAbsoluteUrl(string relativeLocation)
        {
            return _locationTemplateParser.SetBase(relativeLocation, _options.RequestRootUrl);
        }

        private void CreateDirectoryRecursively(SftpClient client, string path)
        {
            string current = "";

            if (path[0] == '/')
            {
                path = path.Substring(1);
            }

            while (!string.IsNullOrEmpty(path))
            {
                int p = path.IndexOf('/');
                current += '/';
                if (p >= 0)
                {
                    current += path.Substring(0, p);
                    path = path.Substring(p + 1);
                }
                else
                {
                    current += path;
                    path = "";
                }

                if (client.Exists(current))
                {
                    var attrs = client.GetAttributes(current);
                    if (!attrs.IsDirectory)
                    {
                        throw new Exception("not directory");
                    }
                }
                else
                {
                    client.CreateDirectory(current);
                }
            }
        }
    }
}
