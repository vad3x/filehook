using System;
using System.Collections.Generic;

namespace Filehook.Metadata
{
    public abstract class EntityMetadata
    {
        private readonly Dictionary<string, PropertyMetadata> _propertyMetadatas = new Dictionary<string, PropertyMetadata>();

        public string Name { get; internal set; }

        public string Postfix { get; internal set; }

        public string StorageName { get; internal set; }

        public PropertyMetadata AddProperty(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_propertyMetadatas.ContainsKey(key))
            {
                throw new ArgumentException($"'{key}' property is already registered");
            }

            var propertyMetadata = new PropertyMetadata();

            _propertyMetadatas.Add(key, propertyMetadata);

            return propertyMetadata;
        }

        public PropertyMetadata FindPropertyMetadata(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_propertyMetadatas.TryGetValue(key, out var propertyMetadata))
            {
                return propertyMetadata;
            }

            return null;
        }
    }

    public class EntityMetadata<TEntity> : EntityMetadata
    {
        public Func<TEntity, string> GetId { get; internal set; }
    }
}
