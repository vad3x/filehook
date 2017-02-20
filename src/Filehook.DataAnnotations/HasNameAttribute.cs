using System;

namespace Filehook.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class HasNameAttribute : Attribute
    {
        public HasNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}