using System;
using System.Linq;
using System.Reflection;
using Filehook.Abstractions;
using Filehook.DataAnnotations;

namespace Filehook.Core
{
    public class AttributeParamNameResolver : IParamNameResolver
    {
        public string Resolve(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }
            
            var name = memberInfo.GetCustomAttributes<HasNameAttribute>()
                .Select(x => x.Name)
                .FirstOrDefault();

            if (name == null)
            {
                name = memberInfo.Name;
            }

            var postfix = memberInfo.GetCustomAttributes<HasPostfixAttribute>()
                .Select(x => x.Postfix)
                .FirstOrDefault();

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