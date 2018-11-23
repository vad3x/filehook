using System;
using System.IO;
using System.Text;
using System.Threading;
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

        private readonly ILogger<SftpStorage> _logger;

        public SftpStorage(
            IOptions<SftpStorageOptions> options,
            ILogger<SftpStorage> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public string Name => _options.Name;

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            var fullPath = _locationTemplateParser.SetRoot(relativeLocation, _options.BasePath);

            using (var client = new SftpClient(_options.HostName, _options.Port, _options.UserName, _options.Password))
            {
                client.Connect();

                return Task.FromResult(client.Exists(fullPath));
            }
        }

        public Task<bool> RemoveFileAsync(string key, CancellationToken cancellationToken = default)
        {
            var fullPath = _locationTemplateParser.SetRoot(relativeLocation, _options.BasePath);

            using (var client = new SftpClient(_options.HostName, _options.Port, _options.UserName, _options.Password))
            {
                client.Connect();

                try
                {
                    client.DeleteFile(fullPath);
                }
                catch (SftpPathNotFoundException)
                {
                    _logger.LogError("File with path '{path}' could not be found", fullPath);

                    return Task.FromResult(false);
                }

                return Task.FromResult(true);
            }
        }

        public async Task<string> OldSaveAsync(string relativeLocation, Stream stream, CancellationToken cancellationToken = default)
        {
            var fullPath = _locationTemplateParser.SetRoot(relativeLocation, _options.BasePath);

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
            return _locationTemplateParser.SetRoot(relativeLocation, _options.RequestRootUrl);
        }

        private void CreateDirectoryRecursively(SftpClient client, string path)
        {
            var current = new StringBuilder();

            if (path[0] == '/')
            {
                path = path.Substring(1);
            }

            while (!string.IsNullOrEmpty(path))
            {
                int p = path.IndexOf('/');
                current.Append('/');
                if (p >= 0)
                {
                    current.Append(path.Substring(0, p));
                    path = path.Substring(p + 1);
                }
                else
                {
                    current.Append(path);
                    path = "";
                }

                var temp = current.ToString();
                if (client.Exists(temp))
                {
                    var attrs = client.GetAttributes(temp);
                    if (!attrs.IsDirectory)
                    {
                        throw new NotImplementedException("not directory");
                    }
                }
                else
                {
                    client.CreateDirectory(temp);
                }
            }
        }

        public Task<string> SaveAsync(string relativeLocation, Stream stream, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<FileStorageSavingResult> SaveAsync(string key, FilehookFileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveFileAsync(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
