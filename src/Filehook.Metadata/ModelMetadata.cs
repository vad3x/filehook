using System;
using System.Collections.Generic;

namespace Filehook.Metadata
{
    public class ModelMetadata
    {
        private Dictionary<Type, EntityMetadata> _entityMetadatas = new Dictionary<Type, EntityMetadata>();

        public EntityMetadata AddType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_entityMetadatas.ContainsKey(type))
            {
                throw new ArgumentException($"'{type}' entity is already registered");
            }

            var entityMetadata = new EntityMetadata();

            _entityMetadatas.Add(type, entityMetadata);

            return entityMetadata;
        }

        public EntityMetadata FindEntityMetadataByType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_entityMetadatas.TryGetValue(type, out var entityMetadata))
            {
                return entityMetadata;
            }

            return null;
        }
    }
}
