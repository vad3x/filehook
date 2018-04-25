using System;
using System.Reflection;
using Filehook.Abstractions;

namespace Filehook.Metadata
{
    public class MetadataEntityIdResolver : IEntityIdResolver
    {
        private readonly ModelMetadata _modelMetadata;

        public MetadataEntityIdResolver(ModelMetadata modelMetadata)
        {
            _modelMetadata = modelMetadata;
        }

        public string Resolve<TEntity>(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var entityMetadata = _modelMetadata.FindEntityMetadata<TEntity>();
            if (entityMetadata == null)
            {
                throw new Exception($"There is no metadata for type '{entity.GetType()}'");
            }

            if (entityMetadata.GetId == null)
            {
                return null;
            }

            return entityMetadata.GetId(entity);
        }
        private string TrimEnd(string source, string value)
        {
            if (value == null || !source.EndsWith(value, StringComparison.Ordinal))
            {
                return source;
            }

            return source.Remove(source.LastIndexOf(value, StringComparison.Ordinal));
        }
    }
}