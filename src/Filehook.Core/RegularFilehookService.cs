using Filehook.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public RegularFilehookService(
            IFileStorageNameResolver fileStorageNameResolver,
            IFileStyleResolver fileStyleResolver,
            IEnumerable<IFileStorage> storages,
            IEnumerable<IFileProccessor> fileProccessors,
            ILocationTemplateParser locationTemplateParser,
            ILocationParamFormatter locationParamFormatter)
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

            _fileStorageNameResolver = fileStorageNameResolver;
            _fileStyleResolver = fileStyleResolver;

            _storages = storages;
            _fileProccessors = fileProccessors;

            _locationTemplateParser = locationTemplateParser;
            _locationParamFormatter = locationParamFormatter;
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

            var filename = GetFilename(entity, propertyExpression);

            var fileExtension = Path.GetExtension(filename);

            var storageName = _fileStorageNameResolver.Resolve(propertyExpression);

            var storage = _storages.FirstOrDefault(s => s.Name == storageName);
            if (storage == null)
            {
                throw new ArgumentException($"Storage with name '{storageName}' has not been registered");
            }

            var className = _locationParamFormatter.Format(entity.GetType().Name);
            var attachmentName = _locationParamFormatter.Format(memberExpression.Member.Name);

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

            var filename = GetFilename(entity, propertyExpression);

            var fileExtension = Path.GetExtension(filename);

            var storageName = _fileStorageNameResolver.Resolve(propertyExpression);

            var storage = _storages.FirstOrDefault(s => s.Name == storageName);
            if (storage == null)
            {
                throw new ArgumentException($"Storage with name '{storageName}' has not been registered");
            }

            var className = _locationParamFormatter.Format(entity.GetType().Name);
            var attachmentName = _locationParamFormatter.Format(memberExpression.Member.Name);

            var relativeLocation = _locationTemplateParser.Parse(
                className: className,
                attachmentName: attachmentName,
                attachmentId: id,
                style: style,
                filename: filename);

            return storage.GetUrl(relativeLocation);
        }

        // TODO tests
        public async Task<IDictionary<string, string>> SaveAsync<TEntity>(
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

            var filename = GetFilename(entity, propertyExpression);
            var fileExtension = Path.GetExtension(filename).TrimStart('.');

            var storageName = _fileStorageNameResolver.Resolve(propertyExpression);

            var storage = _storages.FirstOrDefault(s => s.Name == storageName);
            if (storage == null)
            {
                throw new NotSupportedException($"Storage with name '{storageName}' has not been registered");
            }

            var fileProccessor = _fileProccessors.FirstOrDefault(p => p.CanProccess(fileExtension, bytes));
            if (fileProccessor == null)
            {
                throw new NotSupportedException($"Processor for file '{fileExtension}' has not been registered");
            }

            var styles = _fileStyleResolver.Resolve(propertyExpression);

            var proccessedStreams = fileProccessor.Proccess(bytes, styles);

            var className = _locationParamFormatter.Format(entity.GetType().Name);
            var attachmentName = _locationParamFormatter.Format(memberExpression.Member.Name);

            var locations = new Dictionary<string, string>();
            foreach (var proccessed in proccessedStreams)
            {
                var relativeLocation = _locationTemplateParser.Parse(
                    className: className,
                    attachmentName: attachmentName,
                    attachmentId: id,
                    style: proccessed.Key,
                    filename: filename);

                using (proccessed.Value)
                {
                    await storage.SaveAsync(relativeLocation, proccessed.Value);

                    var url = storage.GetUrl(relativeLocation);

                    locations.Add(proccessed.Key, url);
                }
            }

            return locations;
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

        public bool CanProccess(string fileExtension, byte[] bytes)
        {
            var fileProccessor = _fileProccessors.FirstOrDefault(p => p.CanProccess(fileExtension, bytes));
            if (fileProccessor == null)
            {
                return false;
            }

            return true;
        }
    }
}
