using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Filehook.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Filehook.Storages.S3
{
    public class S3Storage : IFileStorage
    {
        private readonly S3StorageOptions _options;

        private readonly ILocationTemplateParser _locationTemplateParser;

        private readonly IAmazonS3 _amazonS3Client;

        private readonly ILogger _logger;

        public S3Storage(
            IOptions<S3StorageOptions> options,
            ILocationTemplateParser locationTemplateParser,
            IAmazonS3 amazonS3Client,
            ILogger<S3Storage> logger)
        {
            _options = options.Value;
            _amazonS3Client = amazonS3Client;
            _locationTemplateParser = locationTemplateParser;
            _logger = logger;
        }

        public string Name => _options.Name;

        public async Task<bool> ExistsAsync(string relativeLocation)
        {
            var key = _locationTemplateParser.SetBase(relativeLocation, string.Empty).TrimStart('/');
            _logger.LogInformation($"Does exist file: '{key}' on '{_options.BucketName}' bucket");

            var response = await _amazonS3Client.GetObjectAsync(_options.BucketName, key);
            return response.Key != null;
        }

        public string GetUrl(string relativeLocation)
        {
            return ToAbsoluteUrl(relativeLocation);
        }

        public async Task<bool> RemoveAsync(string relativeLocation)
        {
            var key = _locationTemplateParser.SetBase(relativeLocation, string.Empty).TrimStart('/');
            _logger.LogInformation($"Delete file: '{key}' from '{_options.BucketName}' bucket");

            await _amazonS3Client.DeleteAsync(_options.BucketName, key, new Dictionary<string, object>());
            return true;
        }

        public async Task<string> SaveAsync(string relativeLocation, Stream stream)
        {
            var key = _locationTemplateParser.SetBase(relativeLocation, string.Empty).TrimStart('/');
            _logger.LogInformation($"Put file: '{key}' to '{_options.BucketName}' bucket");

            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = stream,
                CannedACL = S3CannedACL.PublicRead
            };

            var result = await _amazonS3Client.PutObjectAsync(request);

            var location = ToAbsoluteUrl(relativeLocation);

            _logger.LogInformation($"Created '{location}'");

            return location;
        }

        private string ToAbsoluteUrl(string relativeLocation)
        {
            if (!string.IsNullOrWhiteSpace(_options.ProxyUri))
            {
                return _locationTemplateParser.SetBase(relativeLocation, _options.ProxyUri);
            }

            var baseLocation = $"{_options.Protocol}://s3-{_options.Region}.{_options.HostName}/{_options.BucketName}";

            return _locationTemplateParser.SetBase(relativeLocation, baseLocation);
        }
    }
}
