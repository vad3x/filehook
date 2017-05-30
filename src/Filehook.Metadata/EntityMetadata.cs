using System;
using System.Collections.Generic;
using System.Reflection;

namespace Filehook.Metadata
{
    public class EntityMetadata
    {
        private Dictionary<MemberInfo, PropertyMetadata> _propertyMetadatas = new Dictionary<MemberInfo, PropertyMetadata>();

        public string Name { get; internal set; }

        public string Postfix { get; internal set; }

        public PropertyMetadata AddProperty(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (_propertyMetadatas.ContainsKey(member))
            {
                throw new ArgumentException($"'{member.Name}' property is already registered");
            }

            var propertyMetadata = new PropertyMetadata();

            _propertyMetadatas.Add(member, propertyMetadata);

            return propertyMetadata;
        }

        public PropertyMetadata FindPropertyMetadata(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (_propertyMetadatas.TryGetValue(member, out var propertyMetadata))
            {
                return propertyMetadata;
            }

            return null;
        }
    }
}