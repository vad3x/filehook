using System;

namespace Filehook.Metadata.Builders
{
    public class PropertyBuilder<TProperty>
    {
        private readonly PropertyMetadata _propertyMetadata;

        public PropertyBuilder(PropertyMetadata propertyMetadata)
        {
            _propertyMetadata = propertyMetadata;
        }

        public PropertyBuilder<TProperty> HasName(string name)
        {
            _propertyMetadata.Name = name ?? throw new ArgumentNullException(nameof(name));

            return this;
        }

        public PropertyBuilder<TProperty> HasPostfix(string postfix)
        {
            _propertyMetadata.Postfix = postfix ?? throw new ArgumentNullException(nameof(postfix));

            return this;
        }

        public PropertyBuilder<TProperty> UseStorage(string storageName)
        {
            _propertyMetadata.StorageName = storageName ?? throw new ArgumentNullException(nameof(storageName));

            return this;
        }
    }
}
