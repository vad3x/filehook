using System;
using System.Collections.Generic;

namespace Filehook.Metadata
{
    public class ModelMetadata
    {
        private Dictionary<string, EntityMetadata> _entityMetadatas = new Dictionary<string, EntityMetadata>();

        public EntityMetadata<TEntity> AddType<TEntity>()
        {
            var key = typeof(TEntity).FullName;

            if (_entityMetadatas.ContainsKey(key))
            {
                throw new ArgumentException($"'{key}' entity is already registered");
            }

            var entityMetadata = new EntityMetadata<TEntity>();

            _entityMetadatas.Add(key, entityMetadata);

            return entityMetadata;
        }

        public EntityMetadata<TEntity> FindEntityMetadata<TEntity>()
        {
            return (EntityMetadata<TEntity>)FindEntityMetadataByFullName(typeof(TEntity).FullName);
        }

        public EntityMetadata FindEntityMetadataByFullName(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_entityMetadatas.TryGetValue(key, out var entityMetadata))
            {
                return entityMetadata;
            }

            return null;
        }
    }
}
