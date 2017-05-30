using System;
using Filehook.Abstractions;
using Filehook.Proccessors.Image.Abstractions;

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

        public PropertyBuilder<TProperty> HasFileStyle(FileStyle fileStyle)
        {
            if (fileStyle == null)
            {
                throw new ArgumentNullException(nameof(fileStyle));
            }

            _propertyMetadata.AddStyle(fileStyle);

            return this;
        }

        public PropertyBuilder<TProperty> HasImageStyle(ImageStyle imageStyle)
        {
            if (imageStyle == null)
            {
                throw new ArgumentNullException(nameof(imageStyle));
            }

            _propertyMetadata.AddStyle(imageStyle);

            return this;
        }
    }
}
