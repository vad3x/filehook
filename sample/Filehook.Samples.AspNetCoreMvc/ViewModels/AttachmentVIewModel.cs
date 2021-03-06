﻿using Filehook.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Filehook.Samples.AspNetCoreMvc.ViewModels
{
    public class AttachmentViewModel
    {
        [Required]
        public int Id { get; set; }

        [FileFormats("jpg", "jpeg", "png")]
        public IFormFile CoverImageFile { get; set; }

        [FileFormats("pdf")]
        [FileSize(MinFileSize = 0, MaxFileSize = 7 * 1024 * 1024, ErrorMessage = "File size must be less than 7MB")]
        public IFormFile AttachmentFile { get; set; }
    }
}
