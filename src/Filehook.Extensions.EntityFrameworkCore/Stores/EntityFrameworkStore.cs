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
using Microsoft.Extensions.Logging;

namespace Filehook.Extensions.EntityFrameworkCore.Stores
{
    public class EntityFrameworkStore : IFilehookStore
    {
        private readonly FilehookDbContext _filehookDbContext;
        private readonly ILogger<EntityFrameworkStore> _logger;

        public EntityFrameworkStore(ILogger<EntityFrameworkStore> logger, FilehookDbContext filehookDbContext)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
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
                .Select(x => x.Blob)
                .Select(x => ToViewModel(x))
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

            var blobEntity = new FilehookBlobEntity
            {
                Key = key,
                FileName = fileName,
                ContentType = contentType,
                ByteSize = byteSize,
                Checksum = checksum,
                Metadata = metadata ?? new Dictionary<string, string>(),
                CreatedAtUtc = DateTime.UtcNow
            };

            _filehookDbContext.FilehookBlobs.Add(blobEntity);
            await _filehookDbContext.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return ToViewModel(blobEntity);
        }

        public async Task<FilehookAttachment> AddAttachmentAsync(
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

            FilehookBlobEntity blobEntity = await _filehookDbContext.FilehookBlobs
                .FirstOrDefaultAsync(x => x.Key == blob.Key)
                .ConfigureAwait(false);

            var attachmentEntity = new FilehookAttachmentEntity
            {
                Name = name,
                EntityId = entityId,
                EntityType = entityType,
                Blob = blobEntity,
                CreatedAtUtc = DateTime.UtcNow
            };

            _filehookDbContext.FilehookAttachments.Add(attachmentEntity);

            await _filehookDbContext.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return ToViewModel(attachmentEntity);
        }

        public async Task<FilehookAttachment> SetAttachmentAsync(
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

            FilehookAttachmentEntity existing = await _filehookDbContext.FilehookAttachments
                .Where(x => x.Name == name && x.EntityId == entityId && x.EntityType == entityType)
                .Include(x => x.Blob)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (existing != null)
            {
                _logger.LogInformation("Existing attachment with id: '{attachmentId}' is about to remove...", existing.Id);
                _filehookDbContext.FilehookAttachments.Remove(existing);
            }

            return await AddAttachmentAsync(name, entityId, entityType, blob, cancellationToken).ConfigureAwait(false);
        }

        // TODO fix mapping entityId - entityType - attachmentNames
        public Task<FilehookAttachment[]> GetAttachmentsAsync(
            string[] entityIds,
            string entityType,
            string[] names = null,
            CancellationToken cancellationToken = default)
        {
            Guard.Argument(entityIds, nameof(entityIds)).NotNull();
            Guard.Argument(entityType, nameof(entityType)).NotNull().NotEmpty();

            IQueryable<FilehookAttachmentEntity> query = _filehookDbContext.FilehookAttachments
                .Where(x => entityIds.Contains(x.EntityId) && x.EntityType == entityType);

            if (names != null)
            {
                query = query.Where(x => names.Contains(x.Name));
            }

            return query
                .Include(x => x.Blob)
                .Select(x => ToViewModel(x))
                .ToArrayAsync(cancellationToken);
        }

        public async Task PurgeAsync(FilehookBlob[] blobs, CancellationToken cancellationToken = default)
        {
            Guard.Argument(blobs, nameof(blobs)).NotNull();

            var blobKeys = blobs.Select(x => x.Key).ToArray();

            FilehookBlobEntity[] blobEntities = await _filehookDbContext.FilehookBlobs
                .Where(x => blobKeys.Contains(x.Key))
                .ToArrayAsync()
                .ConfigureAwait(false);

            _filehookDbContext.FilehookBlobs.RemoveRange(blobEntities);

            await _filehookDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private FilehookBlob ToViewModel(FilehookBlobEntity entity)
        {
            return new FilehookBlob
            {
                Key = entity.Key,
                ByteSize = entity.ByteSize,
                Checksum = entity.Checksum,
                ContentType = entity.ContentType,
                FileName = entity.FileName,
                Metadata = entity.Metadata,
                CreatedAtUtc = entity.CreatedAtUtc
            };
        }

        private FilehookAttachment ToViewModel(FilehookAttachmentEntity entity)
        {
            return new FilehookAttachment
            {
                Blob = ToViewModel(entity.Blob),
                Name = entity.Name,
                EntityId = entity.EntityId,
                EntityType = entity.EntityType,
                CreatedAtUtc = entity.CreatedAtUtc
            };
        }
    }
}
