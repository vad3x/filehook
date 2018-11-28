using System;
using System.Reflection;
using Dawn;

namespace Filehook.Abstractions
{
    public class FilehookAttachmentOptions
    {
        public string IdPropertyName { get; set; } = "Id";

        public Func<object, string> ResolveEntityId { get; set; }
        public Func<Type, string> ResolveEntityType { get; set; }

        public FilehookAttachmentOptions()
        {
            ResolveEntityId = ResolveEntityIdByConvention;
            ResolveEntityType = ResolveEntityTypeByConvention;
        }

        private string ResolveEntityIdByConvention(object entity)
        {
            Guard.Argument(entity, nameof(entity)).NotNull();

            Type entityType = entity.GetType();
            PropertyInfo property = entityType.GetProperty(IdPropertyName);

            var id = property.GetValue(entity).ToString();
            if (id == null)
            {
                throw new FilehookException($"Property named '{IdPropertyName}' is not declared on the entity, pass FilehookAttachmentOptions object.");
            }

            return id;
        }

        private string ResolveEntityTypeByConvention(Type entityType)
        {
            Guard.Argument(entityType, nameof(entityType)).NotNull();

            return entityType.Name;
        }
    }
}
