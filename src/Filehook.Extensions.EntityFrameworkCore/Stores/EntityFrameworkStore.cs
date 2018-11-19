using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dawn;

using Filehook.Abstractions;
using Filehook.Abstractions.Stores;

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

            var blob = new FilehookBlob
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

            var attachment = new FilehookAttachment
            {
                Name = name,
                EntityId = entityId,
                EntityType = entityType,
                Blob = blob,
                CreatedAtUtc = DateTime.UtcNow
            };

            _filehookDbContext.FilehookAttachments.Add(attachment);

            await _filehookDbContext.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return attachment;
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

            var existing = await _filehookDbContext.FilehookAttachments
                .Where(x => x.Name == name && x.EntityId == entityId && x.EntityType == entityType)
                .Include(x => x.Blob)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (existing != null)
            {
                _logger.LogInformation("Existing attachment with id: '{attachmentId}' is about to remove...", existing.Id);
                _filehookDbContext.FilehookAttachments.Remove(existing);
            }

            return await AddAttachmentAsync(name, entityId, entityType, blob, cancellationToken);
        }

        public Task<FilehookAttachment[]> GetAttachmentsAsync(
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
                .Include(x => x.Blob)
                .Cast<FilehookAttachment>()
                .ToArrayAsync(cancellationToken);
        }

        public Task RemoveAsync(FilehookAttachment[] attachments, CancellationToken cancellationToken = default)
        {
            Guard.Argument(attachments, nameof(attachments)).NotNull();

            _filehookDbContext.FilehookAttachments.RemoveRange(attachments);

            return _filehookDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task RemoveAsync(FilehookBlob blob, CancellationToken cancellationToken = default)
        {
            Guard.Argument(blob, nameof(blob)).NotNull();

            _filehookDbContext.FilehookBlobs.Remove(blob);

            return _filehookDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
