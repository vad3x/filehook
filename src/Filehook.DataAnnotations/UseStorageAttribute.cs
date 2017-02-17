using System;

namespace Filehook.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UseStorageAttribute : Attribute
    {
        public UseStorageAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
