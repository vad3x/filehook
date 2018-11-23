using System.Collections.Generic;
using System.IO;
using System.Threading;
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

        private readonly IAmazonS3 _amazonS3Client;

        private readonly ILogger _logger;

        public S3Storage(
            IOptions<S3StorageOptions> options,
            IAmazonS3 amazonS3Client,
            ILogger<S3Storage> logger)
        {
            _options = options.Value;
            _amazonS3Client = amazonS3Client;
            _logger = logger;
        }

        public string Name => _options.Name;

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            var location = _options.RelativeLocation(string.Empty, key).TrimStart('/');

            _logger.LogInformation($"Does exist file: '{location}' on '{_options.BucketName}' bucket");

            GetObjectResponse response = await _amazonS3Client.GetObjectAsync(_options.BucketName, key, cancellationToken)
                .ConfigureAwait(false);

            return response.Key != null;
        }

        public async Task<bool> RemoveFileAsync(string key, CancellationToken cancellationToken = default)
        {
            var location = _options.RelativeLocation(string.Empty, key).TrimStart('/');
            _logger.LogInformation($"Delete file: '{location}' from '{_options.BucketName}' bucket");

            await _amazonS3Client.DeleteAsync(_options.BucketName, key, new Dictionary<string, object>(), cancellationToken);
            return true;
        }

        public async Task<FileStorageSavingResult> SaveAsync(string key, FilehookFileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            var location = _options.RelativeLocation(string.Empty, key).TrimStart('/');
            _logger.LogInformation($"Put file: '{location}' to '{_options.BucketName}' bucket");

            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                InputStream = fileInfo.FileStream,
                CannedACL = S3CannedACL.PublicRead
            };

            await _amazonS3Client.PutObjectAsync(request, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation($"Created '{location}'");

            return location;
        }
    }
}
