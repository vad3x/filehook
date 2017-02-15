using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Filehook.Samples.AspNetCoreMvc.ViewModels
{
    public class AttachmentViewModel
    {
        [Required]
        public int Id { get; set; }

        // TODO file validation
        public IFormFile CoverImageFile { get; set; }

        // TODO file validation
        public IFormFile AttachmentFile { get; set; }
    }
}
