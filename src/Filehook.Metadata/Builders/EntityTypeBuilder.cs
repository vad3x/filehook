using System;
using System.Linq.Expressions;

namespace Filehook.Metadata.Builders
{
    public class EntityTypeBuilder<TEntity> where TEntity : class
    {
        private readonly EntityMetadata<TEntity> _entityMetadata;

        public EntityTypeBuilder(EntityMetadata<TEntity> metadata)
        {
            _entityMetadata = metadata;
        }

        public EntityTypeBuilder<TEntity> HasId(Func<TEntity, string> getIdAction)
        {
            _entityMetadata.GetId = getIdAction ?? throw new ArgumentNullException(nameof(getIdAction));

            return this;
        }

        public EntityTypeBuilder<TEntity> HasName(string name)
        {
            _entityMetadata.Name = name ?? throw new ArgumentNullException(nameof(name));

            return this;
        }

        public EntityTypeBuilder<TEntity> HasStorage(string storageName)
        {
            _entityMetadata.StorageName = storageName ?? throw new ArgumentNullException(nameof(storageName));

            return this;
        }

        public EntityTypeBuilder<TEntity> HasPostfix(string postfix)
        {
            _entityMetadata.Postfix = postfix ?? throw new ArgumentNullException(nameof(postfix));

            return this;
        }

        public PropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
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

            var propertyMetadata = _entityMetadata.AddProperty(memberExpression.Member.Name);

            return new PropertyBuilder<TProperty>(propertyMetadata);
        }
    }
}
