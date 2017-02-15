using Filehook.Core;
using Filehook.Proccessors.Image.Abstractions;

namespace Filehook.Samples.AspNetCoreMvc.Models
{
    public class Article
    {
        public int Id { get; set; }

        [HasImageStyle("small", 0, 200)]
        [HasImageStyle("large", 0, 300)]
        [HasImageStyle("original", 0, 500)]
        public string CoverImageFileName { get; set; }

        public string AttachmentFileName { get; set; }
    }
}
