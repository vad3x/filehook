using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dawn;

using Filehook.Abstractions;
using Filehook.Abstractions.Stores;
using Filehook.Extensions.EntityFrameworkCore.Entities;

using Microsoft.EntityFrameworkCore;

namespace Filehook.Extensions.EntityFrameworkCore.Stores
{
    public class EntityFrameworkStore : IFilehookStore
    {
        private readonly FilehookDbContext _filehookDbContext;

        public EntityFrameworkStore(FilehookDbContext filehookDbContext)
        {
            _filehookDbContext = Guard.Argument(filehookDbContext, nameof(filehookDbContext)).NotNull().Value;
        }

        public Task<FilehookBlob[]> GetBlobsAsync(
            string name,
            string entityId,
            string entityType,
            CancellationToken cancellationToken = default)
        {
            Guard.Argument(name, nameof(name)).NotNull().NotEmpty();
            Guard.Argument(entityId, nameof(entityId)).NotNull().NotEmpty();
            Guard.Argument(entityType, nameof(entityType)).NotNull().NotEmpty();

            return _filehookDbContext.FilehookAttachments
                .Where(x => x.Name == name && x.EntityId == entityId && x.EntityType == entityType)
                .Select(x => (FilehookBlob)x.Blob)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<FilehookBlob> CreateBlobAsync(
            string key,
            string fileName,
            string contentType,
            long byteSize,
            string checksum,
            IDictionary<string, string> metadata = null,
            CancellationToken cancellationToken = default)
        {
            Guard.Argument(key, nameof(key)).NotNull().NotEmpty();
            Guard.Argument(fileName, nameof(fileName)).NotNull().NotEmpty();
            Guard.Argument(contentType, nameof(contentType)).NotNull().NotEmpty();
            Guard.Argument(byteSize, nameof(byteSize)).NotZero().NotNegative();
            Guard.Argument(checksum, nameof(checksum)).NotNull().NotEmpty();

            var blob = new BlobEntity
            {
                Key = key,
                FileName = fileName,
                ContentType = contentType,
                ByteSize = byteSize,
                Checksum = checksum,
                Metadata = metadata ?? new Dictionary<string, string>(),
                CreatedAtUtc = DateTime.UtcNow
            };

            _filehookDbContext.FilehookBlobs.Add(blob);
            await _filehookDbContext.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return blob;
        }

        public async Task<FilehookAttachment> CreateAttachmentAsync(
            string name,
            string entityId,
            string entityType,
            FilehookBlob blob,
            CancellationToken cancellationToken = default)
        {
            Guard.Argument(name, nameof(name)).NotNull().NotEmpty();
            Guard.Argument(entityId, nameof(entityId)).NotNull().NotEmpty();
            Guard.Argument(entityType, nameof(entityType)).NotNull().NotEmpty();
            Guard.Argument(blob, nameof(blob)).NotNull();

            var attachment = new AttachmentEntity
            {
                Name = name,
                EntityId = entityId,
                EntityType = entityType,
                Blob = (BlobEntity)blob,
                CreatedAtUtc = DateTime.UtcNow
            };

            _filehookDbContext.FilehookAttachments.Add(attachment);

            await _filehookDbContext.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return attachment;
        }
    }
}
