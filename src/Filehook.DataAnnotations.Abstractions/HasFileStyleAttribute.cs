using Filehook.Abstractions;
using System;

namespace Filehook.DataAnnotations.Abstractions
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class HasFileStyleAttribute : Attribute
    {
        public HasFileStyleAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Style = new FileStyle(name);
        }

        public FileStyle Style { get; protected set; }
    }
}
