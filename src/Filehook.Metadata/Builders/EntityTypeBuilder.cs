using System;
using System.Linq.Expressions;

namespace Filehook.Metadata.Builders
{
    public class EntityTypeBuilder<TEntity> where TEntity : class
    {
        private readonly EntityMetadata _entityMetadata;

        public EntityTypeBuilder(Type type, EntityMetadata metadata)
        {
            _entityMetadata = metadata;
        }

        public EntityTypeBuilder<TEntity> HasName(string name)
        {
            _entityMetadata.Name = name ?? throw new ArgumentNullException(nameof(name));

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

            var propertyMetadata = _entityMetadata.AddProperty(memberExpression.Member);

            return new PropertyBuilder<TProperty>(propertyMetadata);
        }
    }
}
