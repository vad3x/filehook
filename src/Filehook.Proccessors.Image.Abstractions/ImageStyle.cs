using System;
using Filehook.Abstractions;

namespace Filehook.Proccessors.Image.Abstractions
{
    public class ImageStyle : FileStyle
    {
        public ImageStyle(string name, ImageResizeOptions resizeOptions = null, ImageDecodeOptions decodeOptions = null) : base(name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            ResizeOptions = resizeOptions;
            DecodeOptions = decodeOptions;

            if (resizeOptions == null)
            {
                ResizeOptions = new ImageResizeOptions();
            }

            if (decodeOptions == null)
            {
                DecodeOptions = new ImageDecodeOptions();
            }
        }

        public ImageResizeOptions ResizeOptions { get; set; }

        public ImageDecodeOptions DecodeOptions { get; set; }
    }
}
