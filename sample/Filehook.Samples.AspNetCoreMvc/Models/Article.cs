using Filehook.DataAnnotations;
using Filehook.Proccessors.Image.Abstractions;

namespace Filehook.Samples.AspNetCoreMvc.Models
{
    [HasName("MyArticle")]
    public class Article
    {
        public int Id { get; set; }

        [HasImageStyle("thumb", 300, 0)]
        [HasImageStyle("wide_thumb", 0, 220)]
        [HasImageStyle("retina_thumb", 0, 600)]
        [HasImageStyle("iphone", 640, 1136)]
        [HasImageStyle("ipad", 768, 1024)]
        [HasImageStyle("ipad_retina", 1536, 2048)]
        [HasImageStyle("desktop_hd", 0, 720)]
        [HasImageStyle("desktop_full_hd", 0, 1080, ImageResizeMode.ShrinkLarger)]
        [HasImageStyle("desktop_retina", 0, 1800, ImageResizeMode.ShrinkLarger)]
        [HasPostfix("FileName")]
        public string CoverImageFileName { get; set; }

        [HasName("Attachment")]
        public string AttachmentFileName { get; set; }
    }
}
