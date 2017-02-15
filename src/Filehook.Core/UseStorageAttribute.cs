using System;

namespace Filehook.Core
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
