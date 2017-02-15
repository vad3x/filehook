using System;

namespace Filehook.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class HasFileStyleAttribute : Attribute 
    {
        public HasFileStyleAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public string Name { get; private set; }
    }
}
