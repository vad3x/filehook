using Filehook.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public Task<IEnumerable<FileProccessingResult>> ProccessAsync(byte[] bytes, IEnumerable<FileStyle> styles)
        {
            var result = styles.Select(style => new FileProccessingResult
            {
                Style = style,
                Stream = new MemoryStream(bytes, false)
            });

            return Task.FromResult(result);
        }
    }
}
