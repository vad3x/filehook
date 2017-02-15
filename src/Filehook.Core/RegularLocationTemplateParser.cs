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
            string attachmentName = null,
            string attachmentId = null,
            string style = null,
            string filename = null,
            string locationTemplate = null)
        {
            var location = locationTemplate ?? _options.LocationTemplate;

            if (className != null)
            {
                location = location.Replace(":class", className);
            }

            if (attachmentName != null)
            {
                location = location.Replace(":attachmentName", attachmentName);
            }

            if (attachmentId != null)
            {
                location = location.Replace(":attachmentId", attachmentId);
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

        public string SetBase(string locationTemplate, string baseLocation)
        {
            if (baseLocation == null)
            {
                throw new ArgumentNullException(nameof(baseLocation));
            }

            if (locationTemplate == null)
            {
                throw new ArgumentNullException(nameof(locationTemplate));
            }

            return locationTemplate.Replace(":base", baseLocation);
        }
    }
}
