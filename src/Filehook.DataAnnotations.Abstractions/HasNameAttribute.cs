using System;

namespace Filehook.DataAnnotations.Abstractions
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