using System;
using CaseExtensions;
using Filehook.Abstractions;

namespace Filehook.Core
{
    public class KebabLocationParamFormatter : ILocationParamFormatter
    {
         public string Format(string value)
         {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

             return value.ToKebabCase();
         }
    }
}
