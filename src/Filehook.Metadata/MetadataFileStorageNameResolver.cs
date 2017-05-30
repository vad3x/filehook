using System;
using System.Linq.Expressions;
using Filehook.Abstractions;

namespace Filehook.Metadata
{
    public class MetadataFileStorageNameResolver : IFileStorageNameResolver
    {
        private readonly ModelMetadata _modelMetadata;

        public MetadataFileStorageNameResolver(ModelMetadata modelMetadata)
        {
            _modelMetadata = modelMetadata;
        }

        public string Resolve<TEntity>(Expression<Func<TEntity, string>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException($"'{propertyExpression}': is not a valid expression for this method");
            }

            var storageName = _modelMetadata.FindEntityMetadataByType(memberExpression.Member.DeclaringType)
                ?.FindPropertyMetadata(memberExpression.Member)
                ?.StorageName;

            return storageName;
        }
    }
}
