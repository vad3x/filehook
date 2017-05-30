using System;

namespace Filehook.DataAnnotations.Abstractions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HasPostfixAttribute : Attribute
    {
        public HasPostfixAttribute(string name)
        {
            Postfix = name;
        }

        public string Postfix { get; private set; }
    }
}