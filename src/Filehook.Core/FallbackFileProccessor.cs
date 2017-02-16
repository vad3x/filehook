using Filehook.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Filehook.Core
{
    public class FallbackFileProccessor : IFileProccessor
    {
        private readonly FallbackFileProccessorOptions _options;

        public FallbackFileProccessor(IOptions<FallbackFileProccessorOptions> options)
        {
            _options = options.Value;
        }

        public bool CanProccess(string fileExtension, byte[] bytes)
        {
            if (_options.AllowedExtensions == null)
            {
                return true;
            }

            return _options.AllowedExtensions.Any(e => e == fileExtension);
        }

        public IDictionary<string, MemoryStream> Proccess(byte[] bytes, IEnumerable<FileStyle> styles)
        {
            return styles.ToDictionary(style => style.Name, style => new MemoryStream(bytes, false));
        }
    }
}
