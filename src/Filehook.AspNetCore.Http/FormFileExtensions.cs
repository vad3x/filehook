using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    public static class FormFileExtensions
    {
        public static async Task<byte[]> GetBytesAsync(this IFormFile formFile)
        {
            if (formFile == null)
            {
                throw new ArgumentNullException(nameof(formFile));
            }

            using (var memoryStream = new MemoryStream())
            using (Stream sourceStream = formFile.OpenReadStream())
            {
                await sourceStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                return memoryStream.ToArray();
            }
        }
    }
}
