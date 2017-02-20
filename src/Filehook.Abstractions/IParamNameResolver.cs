using System.Reflection;

namespace Filehook.Abstractions
{
    public interface IParamNameResolver
    {
         string Resolve(MemberInfo memberInfo);
    }
}