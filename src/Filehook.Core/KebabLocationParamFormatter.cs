using System;
using CaseExtensions;
using Filehook.Abstractions;
using Microsoft.Extensions.Options;

namespace Filehook.Core
{
    public class KebabLocationParamFormatter : ILocationParamFormatter
    {
        private readonly KebabLocationParamFormatterOptions _options;

        public KebabLocationParamFormatter(IOptions<KebabLocationParamFormatterOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

         public string Format(string value)
         {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

             return TrimEnd(value, _options.Postfix).ToKebabCase();
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
