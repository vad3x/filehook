using Filehook.Abstractions;

namespace Filehook.Proccessors.Image.Abstractions
{
    public class ImageStyle : FileStyle
    {
        public ImageStyle(string name, ImageResizeOptions resizeOptions = null, ImageDecodeOptions decodeOptions = null) : base(name)
        {
            ResizeOptions = resizeOptions;
            DecodeOptions = decodeOptions;
        }

        public ImageResizeOptions ResizeOptions { get; set; }

        public ImageDecodeOptions DecodeOptions { get; set; }
    }
}
