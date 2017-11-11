using System;
using System.Linq;
using System.Reflection;
using Filehook.Abstractions;
using Filehook.DataAnnotations.Abstractions;

namespace Filehook.DataAnnotations
{
    public class AttributeEntityIdResolver : IEntityIdResolver
    {
        public string Resolve<TEntity>(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var typeInfo = typeof(TEntity).GetTypeInfo();

            var idProperty = typeInfo.DeclaredProperties
                .FirstOrDefault(prop => prop.IsDefined(typeof(HasIdAttribute)));

            if (idProperty == null)
            {
                throw new Exception($"There is no HasIdAttribute for type '{entity.GetType()}'");
            }

            return (string)idProperty.GetValue(entity);
        }
    }
}