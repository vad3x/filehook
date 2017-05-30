using System;
using System.Linq.Expressions;

namespace Filehook.Metadata
{
    public class EntityTypeBuilder<TEntity> where TEntity : class
    {
        public EntityTypeBuilder<TEntity> HasName(string name)
        {
            return this;
        }

        public PropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            return null;
        }
    }
}