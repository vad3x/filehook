using Filehook.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection;

namespace Filehook.Core
{
    public class RegularFilehookService : IFilehookService
    {
        private readonly IFileStorageNameResolver _fileStorageNameResolver;

        private readonly IFileStyleResolver _fileStyleResolver;

        private readonly IEnumerable<IFileStorage> _storages;
        private readonly IEnumerable<IFileProccessor> _fileProccessors;

        private readonly ILocationTemplateParser _locationTemplateParser;
        private readonly ILocationParamFormatter _locationParamFormatter;

        private readonly IParamNameResolver _paramNameResolver;

        public RegularFilehookService(
            IFileStorageNameResolver fileStorageNameResolver,
            IFileStyleResolver fileStyleResolver,
            IEnumerable<IFileStorage> storages,
            IEnumerable<IFileProccessor> fileProccessors,
            ILocationTemplateParser locationTemplateParser,
            ILocationParamFormatter locationParamFormatter,
            IParamNameResolver paramNameResolver)
        {
            if (fileStorageNameResolver == null)
            {
                throw new ArgumentNullException(nameof(fileStorageNameResolver));
            }

            if (fileStyleResolver == null)
            {
                throw new ArgumentNullException(nameof(fileStyleResolver));
            }

            if (storages == null)
            {
                throw new ArgumentNullException(nameof(storages));
            }

            if (fileProccessors == null)
            {
                throw new ArgumentNullException(nameof(fileProccessors));
            }

            if (locationTemplateParser == null)
            {
                throw new ArgumentNullException(nameof(locationTemplateParser));
            }

            if (locationParamFormatter == null)
            {
                throw new ArgumentNullException(nameof(locationParamFormatter));
            }

            if (paramNameResolver == null)
            {
                throw new ArgumentNullException(nameof(paramNameResolver));
            }

            _fileStorageNameResolver = fileStorageNameResolver;
            _fileStyleResolver = fileStyleResolver;

            _storages = storages;
            _fileProccessors = fileProccessors;

            _locationTemplateParser = locationTemplateParser;
            _locationParamFormatter = locationParamFormatter;

            _paramNameResolver = paramNameResolver;
        }

        public Task<bool> ExistsAsync<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string id,
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

            var storage = GetStorage(propertyExpression);

            var filename = GetFilename(entity, propertyExpression);

            var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));
            var attachmentName = _locationParamFormatter.Format(_paramNameResolver.Resolve(memberExpression.Member));

            var relativeLocation = _locationTemplateParser.Parse(
                className: className,
                attachmentName: attachmentName,
                attachmentId: id,
                style: style,
                filename: filename);

            return storage.ExistsAsync(relativeLocation);
        }

        public IDictionary<string, string> GetUrls<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string id) where TEntity : class
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

            return styles.ToDictionary(s => s.Name, s => GetUrl(entity, propertyExpression, id, s.Name));
        }

        // TODO tests
        public string GetUrl<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            string id,
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

            var storage = GetStorage(propertyExpression);

            var filename = GetFilename(entity, propertyExpression);

            var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));
            var attachmentName = _locationParamFormatter.Format(_paramNameResolver.Resolve(memberExpression.Member));

            var relativeLocation = _locationTemplateParser.Parse(
                className: className,
                attachmentName: attachmentName,
                attachmentId: id,
                style: style,
                filename: filename);

            return storage.GetUrl(relativeLocation);
        }

        // TODO tests
        public async Task<IDictionary<string, FilehookSavingResult>> SaveAsync<TEntity>(
            TEntity entity,
            Expression<Func<TEntity, string>> propertyExpression,
            byte[] bytes,
            string id) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
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

            var storage = GetStorage(propertyExpression);

            var filename = GetFilename(entity, propertyExpression);

            var fileExtension = Path.GetExtension(filename).TrimStart('.');
            var fileProccessor = _fileProccessors.FirstOrDefault(p => p.CanProccess(fileExtension, bytes));
            if (fileProccessor == null)
            {
                throw new NotSupportedException($"Processor for file '{fileExtension}' has not been registered");
            }

            var styles = _fileStyleResolver.Resolve(propertyExpression);

            var proccessingResults = await fileProccessor.ProccessAsync(bytes, styles);

            var className = _locationParamFormatter.Format(_paramNameResolver.Resolve(typeof(TEntity).GetTypeInfo()));
            var attachmentName = _locationParamFormatter.Format(_paramNameResolver.Resolve(memberExpression.Member));

            var result = new Dictionary<string, FilehookSavingResult>();
            foreach (var proccessed in proccessingResults)
            {
                var relativeLocation = _locationTemplateParser.Parse(
                    className: className,
                    attachmentName: attachmentName,
                    attachmentId: id,
                    style: proccessed.Style.Name,
                    filename: filename);

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

        private string GetFilename<TEntity>(TEntity entity, Expression<Func<TEntity, string>> propertyExpression)
        {
            var func = ExpressionCache<Func<TEntity, string>>.CachedCompile(propertyExpression);
            var filename = func(entity);
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentException($"'{propertyExpression}': returned invalid string");
            }

            return filename;
        }

        private IFileStorage GetStorage<TEntity>(Expression<Func<TEntity, string>> propertyExpression)
        {
            var storageName = _fileStorageNameResolver.Resolve(propertyExpression);

            var storage = _storages.FirstOrDefault(s => s.Name == storageName);
            if (storage == null)
            {
                throw new NotSupportedException($"Storage with name '{storageName}' has not been registered");
            }

            return storage;
        }
    }
}
