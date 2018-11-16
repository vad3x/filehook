using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using Dawn;

using Filehook.Abstractions;
using Filehook.Abstractions.Stores;

using Microsoft.Extensions.Options;

namespace Filehook.Core
{
    public class RegularNewFilehookService : INewFilehookService
    {
        private readonly FilehookOptions _options;

        private readonly IFileStorageNameResolver _fileStorageNameResolver;

        private readonly IFileStyleResolver _fileStyleResolver;

        private readonly IEnumerable<IFileStorage> _storages;
        private readonly IEnumerable<IFileProccessor> _fileProccessors;

        private readonly ILocationTemplateParser _locationTemplateParser;
        private readonly ILocationParamFormatter _locationParamFormatter;

        private readonly IParamNameResolver _paramNameResolver;
        private readonly IEntityIdResolver _entityIdResolver;
        private readonly IFilehookStore _filehookStore;
        private readonly IEnumerable<IBlobMetadataExtender> _blobMetadataExtenders;

        public RegularNewFilehookService(
            IOptions<FilehookOptions> fileStorageNameResolverOptions,
            IFileStorageNameResolver fileStorageNameResolver,
            IFileStyleResolver fileStyleResolver,
            IEnumerable<IFileStorage> storages,
            IEnumerable<IFileProccessor> fileProccessors,
            ILocationTemplateParser locationTemplateParser,
            ILocationParamFormatter locationParamFormatter,
            IParamNameResolver paramNameResolver,
            IEntityIdResolver entityIdResolver,
            IFilehookStore filehookStore,
            IEnumerable<IBlobMetadataExtender> blobMetadataExtenders)
        {
            if (fileStorageNameResolverOptions == null)
            {
                throw new ArgumentNullException(nameof(fileStorageNameResolverOptions));
            }

            _options = fileStorageNameResolverOptions.Value;
            _fileStorageNameResolver = fileStorageNameResolver ?? throw new ArgumentNullException(nameof(fileStorageNameResolver));
            _fileStyleResolver = fileStyleResolver ?? throw new ArgumentNullException(nameof(fileStyleResolver));

            _storages = storages ?? throw new ArgumentNullException(nameof(storages));
            _fileProccessors = fileProccessors ?? throw new ArgumentNullException(nameof(fileProccessors));

            _locationTemplateParser = locationTemplateParser ?? throw new ArgumentNullException(nameof(locationTemplateParser));
            _locationParamFormatter = locationParamFormatter ?? throw new ArgumentNullException(nameof(locationParamFormatter));

            _paramNameResolver = paramNameResolver ?? throw new ArgumentNullException(nameof(paramNameResolver));
            _entityIdResolver = entityIdResolver ?? throw new ArgumentNullException(nameof(entityIdResolver));
            _filehookStore = Guard.Argument(filehookStore, nameof(filehookStore)).NotNull().Value;
            _blobMetadataExtenders = Guard.Argument(blobMetadataExtenders, nameof(blobMetadataExtenders)).NotNull().Value;
        }

        public Task<FilehookBlob[]> GetBlobsAsync<TEntity>(
            TEntity entity,
            string name,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(name, nameof(name)).NotNull().NotEmpty();
            Guard.Argument(entity, nameof(entity)).NotNull();

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));
            var objectId = _entityIdResolver.Resolve(entity);
            if (objectId == null)
            {
                throw new NotImplementedException("Configuration error, `objectId` has not beed mapped.");
            }

            return _filehookStore.GetBlobsAsync(name, objectId, className, cancellationToken);
        }

        public async Task<FilehookSavingResult> SaveAsync<TEntity>(
            TEntity entity,
            string name,
            FilehookFileInfo fileInfo,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            Guard.Argument(entity, nameof(entity)).NotNull();
            Guard.Argument(name, nameof(name)).NotNull().NotEmpty();

            var objectId = _entityIdResolver.Resolve(entity);
            if (objectId == null)
            {
                throw new ArgumentException($"{nameof(objectId)} is null");
            }

            var storageName = _options.DefaultStorageName;
            var storage = _storages.FirstOrDefault(s => s.Name == storageName);
            if (storage == null)
            {
                throw new NotSupportedException($"Storage with name `{storageName}` has not been registered");
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FileName);
            var fileExtension = Path.GetExtension(fileInfo.FileName).TrimStart('.');

            var key = Guid.NewGuid().ToString("n");

            var newFileName = $"{fileNameWithoutExtension.GenerateSlug()}-{key.Substring(0, 6)}.{fileExtension}";

            var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));
            var locationPropertyName = _locationParamFormatter.Format(name);

            var relativeLocation = _locationTemplateParser.Parse(
                className: className,
                propertyName: locationPropertyName,
                objectId: objectId,
                filename: newFileName);

            var checksum = GetMD5Checksum(fileInfo.FileStream);
            var byteSize = fileInfo.FileStream.Length;

            var metadata = new Dictionary<string, string>();
            foreach (var extender in _blobMetadataExtenders)
            {
                await extender.ExtendAsync(metadata, fileInfo).ConfigureAwait(false);
            }

            var absoluteLocation = await storage.SaveAsync(relativeLocation, fileInfo.FileStream, cancellationToken)
                .ConfigureAwait(false);

            FilehookBlob blob = await _filehookStore.CreateBlobAsync(key, newFileName, fileInfo.ContentType, byteSize, checksum, metadata, cancellationToken)
                .ConfigureAwait(false);

            await _filehookStore.CreateAttachmentAsync(name, objectId, className, blob, cancellationToken)
                .ConfigureAwait(false);

            return FilehookSavingResult.Success(blob, storageName, absoluteLocation);
        }

        private static string GetMD5Checksum(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                stream.Position = 0;

                var hash = md5.ComputeHash(stream);

                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
