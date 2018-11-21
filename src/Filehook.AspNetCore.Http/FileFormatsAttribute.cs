using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Http;

namespace Filehook.AspNetCore.Http
{
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
    public class FileFormatsAttribute : ValidationAttribute
    {
        public FileFormatsAttribute(params string[] formats)
        {
            FileFormats = formats;
        }

        public string[] FileFormats { get; set; }

        public override bool IsValid(object value)
        {
            var formFile = value as IFormFile;
            if (formFile == null)
            {
                return true;
            }

            var fileExtention = Path.GetExtension(formFile.FileName)?.Trim('.');
            if (fileExtention == null || !FileFormats.Any(f => f == fileExtention))
            {
                return false;
            }

            return true;
        }
    }
}
