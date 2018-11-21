using System;

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

            EntityMetadata<TEntity> entityMetadata = _modelMetadata.FindEntityMetadata<TEntity>();
            if (entityMetadata == null)
            {
                throw new NotImplementedException($"There is no metadata for type '{entity.GetType()}'");
            }

            if (entityMetadata.GetId == null)
            {
                return null;
            }

            return entityMetadata.GetId(entity);
        }
    }
}
