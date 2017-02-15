using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Filehook.Samples.AspNetCoreMvc.ViewModels
{
    public class AttachmentViewModel
    {
        [Required]
        public int Id { get; set; }

        [FileExtensions(Extensions = "jpg, jpeg")]
        public IFormFile CoverImageFile { get; set; }

        [FileExtensions(Extensions = "pdf")]
        public IFormFile AttachmentFile { get; set; }
    }
}
