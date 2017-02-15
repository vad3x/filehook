using Filehook.Abstractions;
using Filehook.Proccessors.Image.Abstractions;
using ImageSharp;
using ImageSharp.Formats;
using System;
using System.IO;
using System.Linq;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public class ImageSharpImageProccessor : BaseImageProccessor
    {
        private readonly IImageTransformer _imageTransformer;
        private readonly Configuration _configuration;

        public ImageSharpImageProccessor(
            IImageTransformer imageTransformer,
            IImageStyleResolver imageStyleResolver) : base(imageStyleResolver)
        {
            if (imageTransformer == null)
            {
                throw new ArgumentNullException(nameof(imageTransformer));
            }

            _imageTransformer = imageTransformer;

            _configuration = new Configuration();
            _configuration.AddImageFormat(new JpegFormat());
            _configuration.AddImageFormat(new PngFormat());
            _configuration.AddImageFormat(new GifFormat());
        }

        public override bool CanProccess(string fileExtension, byte[] bytes)
        {
            return _configuration.ImageFormats.Any(f => f.SupportedExtensions.Contains(fileExtension) && f.IsSupportedFileFormat(bytes));
        }

        public override MemoryStream ProccessStyle(byte[] bytes, IFileStyle style)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (style == null)
            {
                return new MemoryStream(bytes, false);
            }

            var outputStream = new MemoryStream();

            using (var image = new ImageSharp.Image(bytes, _configuration))
            {
                _imageTransformer.Transform(image, style as ImageStyle);

                image.Save(outputStream);
            }

            return outputStream;
        }
    }
}
