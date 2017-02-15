using Filehook.Proccessors.Image.Abstractions;
using ImageSharp;
using ImageSharp.Processing;
using System;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public class ImageSharpImageTransformer : IImageTransformer
    {
        public void Transform(ImageSharp.Image image, ImageStyle style)
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
                image.Resize(ToResizeOptions(style.ResizeOptions));
            }

            // TODO other
        }

        public ResizeOptions ToResizeOptions(ImageResizeOptions resizeOptions)
        {
            ResizeMode mode;
            switch (resizeOptions.Mode)
            {
                case ImageResizeMode.PreserveAspectRatio: mode = ResizeMode.Max; break;
                case ImageResizeMode.IgnoreAspectRatio: mode = ResizeMode.Stretch; break;
                case ImageResizeMode.ShrinkLarger: mode = ResizeMode.Min; break;
                case ImageResizeMode.Fill: mode = ResizeMode.Min; break;
                default: throw new NotImplementedException();
            }

            var resizeOptionsResult = new ResizeOptions
            {
                Size = new Size(resizeOptions.Width, resizeOptions.Height),
                Mode = mode
            };

            return resizeOptionsResult;
        }
    }
}
