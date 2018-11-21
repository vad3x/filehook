using System;
using Filehook.Abstractions;
using Microsoft.Extensions.Options;

namespace Filehook.Core
{
    public class RegularLocationTemplateParser : ILocationTemplateParser
    {
        private readonly RegularLocationTemplateParserOptions _options;

        public RegularLocationTemplateParser(IOptions<RegularLocationTemplateParserOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

        public string Parse(
            string className = null,
            string propertyName = null,
            string objectId = null,
            string style = null,
            string filename = null,
            string locationTemplate = null)
        {
            var location = locationTemplate ?? _options.LocationTemplate;

            if (className != null)
            {
                location = location.Replace(":objectClass", className);
            }

            if (propertyName != null)
            {
                location = location.Replace(":propertyName", propertyName);
            }

            if (objectId != null)
            {
                location = location.Replace(":objectId", objectId);
            }

            if (style != null)
            {
                location = location.Replace(":style", style);
            }

            if (filename != null)
            {
                location = location.Replace(":filename", filename);
            }

            return location;
        }

        public string SetRoot(string locationTemplate, string rootLocation)
        {
            if (rootLocation == null)
            {
                throw new ArgumentNullException(nameof(rootLocation));
            }

            if (locationTemplate == null)
            {
                throw new ArgumentNullException(nameof(locationTemplate));
            }

            return locationTemplate.Replace(":root", rootLocation);
        }
    }
}
