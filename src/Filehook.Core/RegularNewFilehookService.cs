using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        private readonly IBlobStore _blobStore;
        private readonly IAttachmentStore _attachmentStore;
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
            IBlobStore blobStore,
            IAttachmentStore attachmentStore,
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
            _blobStore = Guard.Argument(blobStore, nameof(blobStore)).NotNull().Value;
            _attachmentStore = Guard.Argument(attachmentStore, nameof(attachmentStore)).NotNull().Value;
            _blobMetadataExtenders = Guard.Argument(blobMetadataExtenders, nameof(blobMetadataExtenders)).NotNull().Value;
        }

        public Task<FilehookBlob[]> GetBlobsAsync<TEntity>(
            TEntity entity,
            string name) where TEntity : class
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

            return _attachmentStore.GetBlobsAsync(name, objectId, className);
        }

        public async Task<FilehookBlob> SaveAsync<TEntity>(TEntity entity, string name, FilehookFileInfo fileInfo) where TEntity : class
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

            var newFileName = $"{fileNameWithoutExtension.GenerateSlug()}.{fileExtension}";

            var key = Guid.NewGuid().ToString("n");

            var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));

            var relativeLocation = _locationTemplateParser.Parse(
                className: className,
                propertyName: name,
                objectId: objectId,
                style: "original",
                filename: newFileName);

            // TODO get metadata

            var absoluteLocation = await storage.SaveAsync(relativeLocation, fileInfo.FileStream)
                .ConfigureAwait(false);

            // TODO
            FilehookBlob blob = await _blobStore.CreateAsync(key, newFileName, fileInfo.ContentType, 0, "")
                .ConfigureAwait(false);

            //var fileProccessor = _fileProccessors.FirstOrDefault(p => p.CanProccess(fileExtension, bytes));
            //if (fileProccessor == null)
            //{
            //    throw new NotSupportedException($"Processor for file '{fileExtension}' has not been registered");
            //}

            //var styles = _fileStyleResolver.Resolve(propertyExpression);

            //var proccessingResults = await fileProccessor.ProccessAsync(bytes, styles);

            //var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));
            //var propertyName = _locationParamFormatter.Format(_paramNameResolver.Resolve(memberExpression.Member));

            //var oldFilename = GetFilename(entity, propertyExpression);

            //var result = new Dictionary<string, FilehookSavingResult>();
            //foreach (var proccessed in proccessingResults)
            //{
            //    var relativeLocation = _locationTemplateParser.Parse(
            //        className: className,
            //        propertyName: propertyName,
            //        objectId: objectId,
            //        style: proccessed.Style.Name,
            //        filename: newFilename);

            //    using (proccessed.Stream)
            //    {
            //        var absoluteLocation = await storage.SaveAsync(relativeLocation, proccessed.Stream);

            //        var url = storage.GetUrl(relativeLocation);

            //        result.Add(proccessed.Style.Name, new FilehookSavingResult
            //        {
            //            Location = absoluteLocation,
            //            Url = url,
            //            ProccessingMeta = proccessed.Meta
            //        });
            //    }

            //    if (!string.IsNullOrWhiteSpace(oldFilename) && oldFilename != newFilename)
            //    {
            //        var oldRelativeLocation = _locationTemplateParser.Parse(
            //            className: className,
            //            propertyName: propertyName,
            //            objectId: objectId,
            //            style: proccessed.Style.Name,
            //            filename: oldFilename);

            //        await storage.RemoveAsync(oldRelativeLocation);
            //    }
            //}

            //if (memberExpression.Member is PropertyInfo propertyInfo)
            //{
            //    propertyInfo.SetValue(entity, newFilename);
            //}

            //return result;
        }

        public IDictionary<string, string> GetUrls<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var styles = _fileStyleResolver.Resolve(propertyExpression);

            return styles.ToDictionary(s => s.Name, s => GetUrl(entity, propertyExpression, s.Name));
        }

        // TODO tests
        public string GetUrl<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string style) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException($"'{propertyExpression}': is not a valid expression for this method");
            }

            var objectId = _entityIdResolver.Resolve(entity);
            if (objectId == null)
            {
                throw new ArgumentException($"{nameof(objectId)} is null");
            }

            var storage = GetStorage(propertyExpression);

            var filename = GetFilename(entity, propertyExpression);

            var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));
            var propertyName = _locationParamFormatter.Format(_paramNameResolver.Resolve(memberExpression.Member));

            var relativeLocation = _locationTemplateParser.Parse(
                className: className,
                propertyName: propertyName,
                objectId: objectId,
                style: style,
                filename: filename);

            return storage.GetUrl(relativeLocation);
        }

        // TODO tests
        public async Task<IDictionary<string, FilehookSavingResult>> SaveAsync<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string filename,
            byte[] bytes) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (filename == null)
            {
                throw new ArgumentNullException(nameof(filename));
            }

            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException($"'{propertyExpression}': is not a valid expression for this method");
            }

            var objectId = _entityIdResolver.Resolve(entity);
            if (objectId == null)
            {
                throw new ArgumentException($"{nameof(objectId)} is null");
            }

            var storage = GetStorage(propertyExpression);

            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            var fileExtension = Path.GetExtension(filename).TrimStart('.');

            var newFilename = $"{filenameWithoutExtension.GenerateSlug()}.{fileExtension}";

            var fileProccessor = _fileProccessors.FirstOrDefault(p => p.CanProccess(fileExtension, bytes));
            if (fileProccessor == null)
            {
                throw new NotSupportedException($"Processor for file '{fileExtension}' has not been registered");
            }

            var styles = _fileStyleResolver.Resolve(propertyExpression);

            var proccessingResults = await fileProccessor.ProccessAsync(bytes, styles);

            var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));
            var propertyName = _locationParamFormatter.Format(_paramNameResolver.Resolve(memberExpression.Member));

            var oldFilename = GetFilename(entity, propertyExpression);

            var result = new Dictionary<string, FilehookSavingResult>();
            foreach (var proccessed in proccessingResults)
            {
                var relativeLocation = _locationTemplateParser.Parse(
                    className: className,
                    propertyName: propertyName,
                    objectId: objectId,
                    style: proccessed.Style.Name,
                    filename: newFilename);

                using (proccessed.Stream)
                {
                    var absoluteLocation = await storage.SaveAsync(relativeLocation, proccessed.Stream);

                    var url = storage.GetUrl(relativeLocation);

                    result.Add(proccessed.Style.Name, new FilehookSavingResult
                    {
                        Location = absoluteLocation,
                        Url = url,
                        ProccessingMeta = proccessed.Meta
                    });
                }

                if (!string.IsNullOrWhiteSpace(oldFilename) && oldFilename != newFilename)
                {
                    var oldRelativeLocation = _locationTemplateParser.Parse(
                        className: className,
                        propertyName: propertyName,
                        objectId: objectId,
                        style: proccessed.Style.Name,
                        filename: oldFilename);

                    await storage.RemoveAsync(oldRelativeLocation);
                }
            }

            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(entity, newFilename);
            }

            return result;
        }

        public bool CanProccess(string fileExtension, byte[] bytes)
        {
            var fileProccessor = _fileProccessors.FirstOrDefault(p => p.CanProccess(fileExtension, bytes));
            if (fileProccessor == null)
            {
                return false;
            }

            return true;
        }

        // TODO tests
        public async Task RemoveAsync<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException($"'{propertyExpression}': is not a valid expression for this method");
            }

            var objectId = _entityIdResolver.Resolve(entity);
            if (objectId == null)
            {
                throw new ArgumentException($"{nameof(objectId)} is null");
            }

            var storage = GetStorage(propertyExpression);

            var filename = GetFilename(entity, propertyExpression);

            var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));
            var propertyName = _locationParamFormatter.Format(_paramNameResolver.Resolve(memberExpression.Member));

            var styles = _fileStyleResolver.Resolve(propertyExpression);

            foreach (var style in styles)
            {
                var relativeLocation = _locationTemplateParser.Parse(
                    className: className,
                    propertyName: propertyName,
                    objectId: objectId,
                    style: style.Name,
                    filename: filename);

                await storage.RemoveAsync(relativeLocation);
            }

            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(entity, null);
            }
        }

        private string GetFilename<TEntity>(TEntity entity, Expression<Func<TEntity, string>> propertyExpression)
        {
            var func = ExpressionCache<Func<TEntity, string>>.CachedCompile(propertyExpression);
            var filename = func(entity);

            return filename;
        }

        private IFileStorage GetStorage<TEntity>(Expression<Func<TEntity, string>> propertyExpression)
        {
            var storageName = _fileStorageNameResolver.Resolve(propertyExpression);

            if (storageName == null)
            {
                storageName = _options.DefaultStorageName;
            }

            var storage = _storages.FirstOrDefault(s => s.Name == storageName);
            if (storage == null)
            {
                throw new NotSupportedException($"Storage with name '{storageName}' has not been registered");
            }

            return storage;
        }
    }
}
