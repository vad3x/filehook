using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Filehook.AspNetCore.Http
{
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
    public class FileSizeAttribute : ValidationAttribute
    {
        public long MinFileSize { get; set; }

        public long MaxFileSize { get; set; }

        public override bool IsValid(object value)
        {
            var formFile = value as IFormFile;
            if (formFile == null)
            {
                return true;
            }

            if (formFile.Length < MinFileSize || formFile.Length > MaxFileSize)
            {
                return false;
            }

            return true;
        }
    }
}
