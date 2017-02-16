using Filehook.Abstractions;

namespace Filehook.Proccessors.Image.Abstractions
{
    public class ImageStyle : FileStyle
    {
        public ImageStyle(string name, ImageResizeOptions resizeOptions = null) : base(name)
        {
            ResizeOptions = resizeOptions;
        }

        public ImageResizeOptions ResizeOptions { get; set; }
    }
}
