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

            string storageName = null;
            var entityMetadata = _modelMetadata.FindEntityMetadataByFullName(memberExpression.Member.DeclaringType.FullName);

            if (entityMetadata != null)
            {
                storageName = entityMetadata.StorageName;

                var propertyMetadata = entityMetadata.FindPropertyMetadata(memberExpression.Member.Name);

                if (propertyMetadata?.StorageName != null)
                {
                    storageName = propertyMetadata.StorageName;
                }
            }

            return storageName;
        }
    }
}
