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
            using (var sourceStream = formFile.OpenReadStream())
            {
                await sourceStream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                return bytes;
            }
        }
    }