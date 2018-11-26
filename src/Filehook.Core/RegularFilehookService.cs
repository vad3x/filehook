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

namespace Filehook.Core
{
    public class RegularFilehookService : IFilehookService
    {
        private readonly FilehookOptions _options;

        private readonly ILogger<RegularFilehookService> _logger;
        private readonly IEnumerable<IFileStorage> _storages;
        private readonly IEntityIdResolver _entityIdResolver;
        private readonly IFilehookStore _filehookStore;
        private readonly IEnumerable<IBlobAnalyzer> _blobAnalyzers;

        public RegularFilehookService(
            ILogger<RegularFilehookService> logger,
            IOptions<FilehookOptions> options,
            IEnumerable<IFileStorage> storages,
            IEntityIdResolver entityIdResolver,
            IFilehookStore filehookStore,
            IEnumerable<IBlobAnalyzer> blobAnalyzers)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;

            _storages = storages ?? throw new ArgumentNullException(nameof(storages));
            _entityIdResolver = entityIdResolver ?? throw new ArgumentNullException(nameof(entityIdResolver));
            _filehookStore = Guard.Argument(filehookStore, nameof(filehookStore)).NotNull().Value;
            _blobAnalyzers = Guard.Argument(blobAnalyzers, nameof(blobAnalyzers)).NotNull().Value;
        }

        public async Task<FilehookUploadingResult> UploadAsync(
            FilehookFileInfo fileInfo,
            CancellationToken cancellationToken = default)
        {
            Guard.Argument(fileInfo, nameof(fileInfo)).NotNull();

            string storageName = _options.DefaultStorageName;
            IFileStorage storage = _storages.FirstOrDefault(s => s.Name == storageName);
            if (storage == null)
            {
                throw new NotSupportedException($"Storage with name '{storageName}' has not been registered");
            }

            var key = _options.NewKey();

            var metadata = new Dictionary<string, string>();
            foreach (IBlobAnalyzer analyzer in _blobAnalyzers)
            {
                await analyzer.AnalyzeAsync(metadata, fileInfo).ConfigureAwait(false);
            }

            FileStorageSavingResult fileSavingResult = await storage.SaveAsync(key, fileInfo, cancellationToken)
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

            return FilehookUploadingResult.Success(blob, storageName, fileSavingResult.AbsoluteLocation);
        }

        public async Task<FilehookAttachment> AttachAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookBlob blob,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(attachmentName, nameof(attachmentName)).NotNull().NotEmpty();
            Guard.Argument(blob, nameof(blob)).NotNull();

            string entityType = typeof(TEntity).Name;
            string entityId = _entityIdResolver.Resolve(entity);
            if (entityId == null)
            {
                throw new ArgumentException($"{nameof(entityId)} is null");
            }

            _logger.LogInformation("Attaching blob: '{blobKey}' to '{attachmentName}' on '{entityType}':'{entityId}' ...", blob.Key, attachmentName, entityType, entityId);

            return await _filehookStore.AddAttachmentAsync(attachmentName, entityId, entityType, blob, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<FilehookAttachment> SetOneAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookFileInfo fileInfo,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(attachmentName, nameof(attachmentName)).NotNull().NotEmpty();
            Guard.Argument(fileInfo, nameof(fileInfo)).NotNull();

            string entityType = typeof(TEntity).Name;
            string entityId = _entityIdResolver.Resolve(entity);
            if (entityId == null)
            {
                throw new ArgumentException($"{nameof(entityId)} is null");
            }

            FilehookAttachment[] existing = await _filehookStore.GetAttachmentsAsync(new[] { entityId }, entityType, new[] { attachmentName }, cancellationToken)
                .ConfigureAwait(false);

            if (existing.Length > 0)
            {
                foreach (FilehookAttachment item in existing)
                {
                    _logger.LogInformation("Purging blob: '{blobKey}'...", item.Blob.Key);

                    await PurgeAsync(new[] { item.Blob }, cancellationToken).ConfigureAwait(false);
                }
            }

            _logger.LogInformation("Uploading file: '{fileName}'...", fileInfo.FileName);
            FilehookUploadingResult fileUploadingResult = await UploadAsync(fileInfo, cancellationToken).ConfigureAwait(false);

            return await AttachAsync(entity, attachmentName, fileUploadingResult.Blob, cancellationToken).ConfigureAwait(false);
        }

        public async Task<FilehookAttachment> AddManyAsync<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookFileInfo fileInfo,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(attachmentName, nameof(attachmentName)).NotNull().NotEmpty();
            Guard.Argument(fileInfo, nameof(fileInfo)).NotNull();

            _logger.LogInformation("Uploading file: '{fileName}'...", fileInfo.FileName);
            FilehookUploadingResult fileUploadingResult = await UploadAsync(fileInfo, cancellationToken).ConfigureAwait(false);

            return await AttachAsync(entity, attachmentName, fileUploadingResult.Blob, cancellationToken).ConfigureAwait(false);
        }

        public async Task PurgeAsync(FilehookBlob[] blobs, CancellationToken cancellationToken = default)
        {
            Guard.Argument(blobs, nameof(blobs)).NotNull();

            await _filehookStore.PurgeAsync(blobs, cancellationToken)
                .ConfigureAwait(false);

            string storageName = _options.DefaultStorageName;
            IFileStorage storage = _storages.FirstOrDefault(s => s.Name == storageName);
            if (storage == null)
            {
                throw new NotSupportedException($"Storage with name '{storageName}' has not been registered");
            }

            foreach (FilehookBlob blob in blobs)
            {
                await storage.RemoveFileAsync(blob.Key).ConfigureAwait(false);
            }
        }

        public Task<FilehookAttachment[]> GetAttachmentsAsync<TEntity>(
            TEntity[] entities,
            string[] attachmentNames = null,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entities, nameof(entities)).NotNull();

            var entityIds = entities.Select(x =>
            {
                string entityId = _entityIdResolver.Resolve(x);
                if (entityId == null)
                {
                    throw new ArgumentException($"{nameof(entityId)} is null");
                }

                return entityId;
            })
            .ToArray();

            string entityType = typeof(TEntity).Name;

            return _filehookStore.GetAttachmentsAsync(entityIds, entityType, attachmentNames, cancellationToken);
        }

        public FilehookBlob GetSingleBlob<TEntity>(TEntity entity, string attachmentName, FilehookAttachment[] attachments) where TEntity : class
        {
            return GetManyBlobs(entity, attachmentName, attachments).FirstOrDefault();
        }

        public FilehookBlob[] GetManyBlobs<TEntity>(
            TEntity entity,
            string attachmentName,
            FilehookAttachment[] attachments) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(attachmentName, nameof(attachmentName)).NotNull().NotEmpty();
            Guard.Argument(attachments, nameof(attachments)).NotNull();

            string entityType = typeof(TEntity).Name;
            string entityId = _entityIdResolver.Resolve(entity);
            if (entityId == null)
            {
                throw new ArgumentException($"{nameof(entityId)} is null");
            }

            return attachments
                .Where(x => x.Name == attachmentName && x.EntityId == entityId && x.EntityType == entityType)
                .Select(x => x.Blob)
                .ToArray();
        }

        public async Task PurgeAsync<TEntity>(
            TEntity entity,
            string attachmentName = null,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();

            var names = attachmentName != null ? new[] { attachmentName } : null;

            FilehookAttachment[] attachments = await GetAttachmentsAsync(new[] { entity }, names, cancellationToken)
                .ConfigureAwait(false);

            FilehookBlob[] blobs = attachments.Select(a => a.Blob).ToArray();

            await PurgeAsync(blobs, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
