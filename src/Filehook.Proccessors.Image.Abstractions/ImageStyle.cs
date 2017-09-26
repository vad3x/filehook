using System;
using Filehook.Abstractions;

namespace Filehook.Proccessors.Image.Abstractions
{
    public class ImageStyle : FileStyle
    {
        public ImageStyle(string name, ImageResizeOptions resizeOptions = null, ImageEncodeOptions decodeOptions = null) : base(name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            ResizeOptions = resizeOptions;
            EncodeOptions = decodeOptions;

            if (resizeOptions == null)
            {
                ResizeOptions = new ImageResizeOptions();
            }

            if (decodeOptions == null)
            {
                EncodeOptions = new ImageEncodeOptions();
            }
        }

        public ImageResizeOptions ResizeOptions { get; set; }

        public ImageEncodeOptions EncodeOptions { get; set; }
    }
}
