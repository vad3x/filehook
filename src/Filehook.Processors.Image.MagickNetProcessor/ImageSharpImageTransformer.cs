using System;
using Filehook.Proccessors.Image.Abstractions;
using ImageMagick;

namespace Filehook.Proccessors.Image.MagickNetProccessor
{
    public class MagickNetImageTransformer : IImageTransformer
    {
        public void Transform(MagickImage image, ImageStyle style)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            if (style.ResizeOptions != null)
            {
                image.Resize(ToResizeOptions(image, style.ResizeOptions));
            }

            // TODO other
        }

        private MagickGeometry ToResizeOptions(MagickImage image, ImageResizeOptions resizeOptions)
        {
            var geometry = new MagickGeometry(resizeOptions.Width, resizeOptions.Height);

            switch (resizeOptions.Mode)
            {
                case ImageResizeMode.PreserveAspectRatio: geometry.IgnoreAspectRatio = false; break;
                case ImageResizeMode.IgnoreAspectRatio: geometry.IgnoreAspectRatio = true; break;
                case ImageResizeMode.ShrinkLarger: geometry.Greater = true; break;
                case ImageResizeMode.Fill: geometry.FillArea = true; break;
                default: throw new NotImplementedException();
            }

            return geometry;
        }
    }
}
