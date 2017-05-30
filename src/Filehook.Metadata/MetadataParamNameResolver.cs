using System;
using System.Reflection;
using Filehook.Abstractions;

namespace Filehook.Metadata
{
    public class MetadataParamNameResolver : IParamNameResolver
    {
        private readonly ModelMetadata _modelMetadata;

        public MetadataParamNameResolver(ModelMetadata modelMetadata)
        {
            _modelMetadata = modelMetadata;
        }

        public string Resolve(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            string name;
            string postfix;
            if (memberInfo.DeclaringType == null)
            {
                var entityMetadata = _modelMetadata.FindEntityMetadataByType(memberInfo.GetType());
                name = entityMetadata?.Name;
                postfix = entityMetadata?.Postfix;
            }
            else
            {
                var propertyMetadata = _modelMetadata.FindEntityMetadataByType(memberInfo.DeclaringType)?.FindPropertyMetadata(memberInfo);
                name = propertyMetadata.Name;
                postfix = propertyMetadata.Postfix;
            }

            if (name == null)
            {
                name = memberInfo.Name;
            }

            if (postfix != null)
            {
                name = TrimEnd(name, postfix);
            }

            return name;
        }

        private string TrimEnd(string source, string value)
        {
            if (value == null || !source.EndsWith(value, StringComparison.Ordinal))
            {
                return source;
            }

            return source.Remove(source.LastIndexOf(value, StringComparison.Ordinal));
        }
    }
}