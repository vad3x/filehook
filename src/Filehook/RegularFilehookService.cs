using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dawn;

using Filehook.Abstractions;
using Filehook.Abstractions.Stores;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Filehook
{
    public class RegularFilehookService : IFilehookService
    {
        private readonly FilehookOptions _options;

        private readonly ILogger<RegularFilehookService> _logger;
        private readonly IFileStorage _storage;
        private readonly IFilehookStore _filehookStore;
        private readonly IEnumerable<IBlobAnalyzer> _blobAnalyzers;

        public RegularFilehookService(
            ILogger<RegularFilehookService> logger,
            IOptions<FilehookOptions> options,
            IFileStorage storage,
            IFilehookStore filehookStore,
            IEnumerable<IBlobAnalyzer> blobAnalyzers)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;

            _storage = Guard.Argument(storage, nameof(storage)).NotNull().Value;
            _filehookStore = Guard.Argument(filehookStore, nameof(filehookStore)).NotNull().Value;
            _blobAnalyzers = Guard.Argument(blobAnalyzers, nameof(blobAnalyzers)).NotNull().Value;
        }

        public async Task<FilehookAttachment> AttachAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookBlob blob,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(attachmentName, nameof(attachmentName)).NotNull().NotEmpty();
            Guard.Argument(blob, nameof(blob)).NotNull();

            filehookAttachmentOptions = filehookAttachmentOptions ?? _options.AttachmentOptions;

            string entityType = filehookAttachmentOptions.ResolveEntityType(entity.GetType());
            string entityId = filehookAttachmentOptions.ResolveEntityId(entity);

            _logger.LogInformation("Attaching blob: '{blobKey}' to '{attachmentName}' on '{entityType}':'{entityId}' ...", blob.Key, attachmentName, entityType, entityId);

            return await _filehookStore.AddAttachmentAsync(attachmentName, new EntityMetadata(entityId, entityType), blob, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<FilehookAttachment> SetAttachmentAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookFileInfo fileInfo,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(attachmentName, nameof(attachmentName)).NotNull().NotEmpty();
            Guard.Argument(fileInfo, nameof(fileInfo)).NotNull();

            filehookAttachmentOptions = filehookAttachmentOptions ?? _options.AttachmentOptions;

            string entityType = filehookAttachmentOptions.ResolveEntityType(entity.GetType());
            string entityId = filehookAttachmentOptions.ResolveEntityId(entity);

            FilehookAttachment[] existing = await _filehookStore.GetAttachmentsAsync(new[] { new EntityMetadata(entityId, entityType) }, new[] { attachmentName }, cancellationToken)
                .ConfigureAwait(false);

            if (existing.Length > 0)
            {
                foreach (FilehookAttachment item in existing)
                {
                    _logger.LogInformation("Purging blob: '{blobKey}'...", item.Blob.Key);

                    await PurgeAsync(new[] { item.Blob }, cancellationToken).ConfigureAwait(false);
                }
            }

            return await AddAttachmentAsync(entity, attachmentName, fileInfo, filehookAttachmentOptions, cancellationToken).ConfigureAwait(false);
        }

        public async Task<FilehookAttachment> AddAttachmentAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookFileInfo fileInfo,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(attachmentName, nameof(attachmentName)).NotNull().NotEmpty();
            Guard.Argument(fileInfo, nameof(fileInfo)).NotNull();

            _logger.LogInformation("Uploading file: '{fileName}'...", fileInfo.FileName);
            FilehookUploadingResult fileUploadingResult = await UploadAsync(fileInfo, cancellationToken).ConfigureAwait(false);

            return await AttachAsync(entity, attachmentName, fileUploadingResult.Blob, filehookAttachmentOptions, cancellationToken).ConfigureAwait(false);
        }

        public async Task PurgeAsync(FilehookBlob[] blobs, CancellationToken cancellationToken = default)
        {
            Guard.Argument(blobs, nameof(blobs)).NotNull();

            await _filehookStore.PurgeAsync(blobs, cancellationToken)
                .ConfigureAwait(false);

            foreach (FilehookBlob blob in blobs)
            {
                await _storage.RemoveFileAsync(blob.Key).ConfigureAwait(false);
            }
        }

        public Task<FilehookAttachment[]> GetAttachmentsAsync<TEntity>(
            TEntity[] entities,
            string[] attachmentNames = null,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entities, nameof(entities)).NotNull();

            filehookAttachmentOptions = filehookAttachmentOptions ?? _options.AttachmentOptions;

            EntityMetadata[] entityMetadatas = entities.Select(x =>
            {
                string entityId = filehookAttachmentOptions.ResolveEntityId(x);
                if (entityId == null)
                {
                    throw new ArgumentException($"{nameof(entityId)} is null");
                }

                string entityType = filehookAttachmentOptions.ResolveEntityType(typeof(TEntity));

                return new EntityMetadata(entityId, entityType);
            })
            .ToArray();

            return _filehookStore.GetAttachmentsAsync(entityMetadatas, attachmentNames, cancellationToken);
        }

        public async Task PurgeAsync<TEntity>(
            TEntity entity,
            string attachmentName = null,
            FilehookAttachmentOptions filehookAttachmentOptions = null,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();

            var names = attachmentName != null ? new[] { attachmentName } : null;

            FilehookAttachment[] attachments = await GetAttachmentsAsync(new[] { entity }, names, filehookAttachmentOptions, cancellationToken)
                .ConfigureAwait(false);

            FilehookBlob[] blobs = attachments.Select(a => a.Blob).ToArray();

            await PurgeAsync(blobs, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<FilehookUploadingResult> UploadAsync(
            FilehookFileInfo fileInfo,
            CancellationToken cancellationToken = default)
        {
            Guard.Argument(fileInfo, nameof(fileInfo)).NotNull();

            var key = _options.NewKey();

            var metadata = new Dictionary<string, string>();
            foreach (IBlobAnalyzer analyzer in _blobAnalyzers)
            {
                await analyzer.AnalyzeAsync(metadata, fileInfo).ConfigureAwait(false);
            }

            FileStorageSavingResult fileSavingResult = await _storage.SaveAsync(key, fileInfo, cancellationToken)
                .ConfigureAwait(false);

            FilehookBlob blob = await _filehookStore.CreateBlobAsync(
                key,
                fileInfo.FileName,
                fileInfo.ContentType,
                fileSavingResult.ByteSize,
                fileSavingResult.Checksum,
                metadata,
                cancellationToken)
                .ConfigureAwait(false);

            return FilehookUploadingResult.Success(blob, fileSavingResult.AbsoluteLocation);
        }

    }
}
