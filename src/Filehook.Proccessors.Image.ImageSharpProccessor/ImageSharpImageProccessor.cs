using Filehook.Abstractions;
using Filehook.Proccessors.Image.Abstractions;
using ImageSharp;
using ImageSharp.Formats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public class ImageSharpImageProccessor : IFileProccessor
    {
        private readonly IImageTransformer _imageTransformer;
        private readonly Configuration _configuration;

        public ImageSharpImageProccessor(IImageTransformer imageTransformer)
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

        public bool CanProccess(string fileExtension, byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return _configuration.ImageFormats.Any(f => f.IsSupportedFileFormat(bytes));
        }

        // TODO async
        public IDictionary<string, MemoryStream> Proccess(byte[] bytes, IEnumerable<FileStyle> styles)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (styles == null)
            {
                throw new ArgumentNullException(nameof(styles));
            }

            Debug.WriteLine($"processing started ...");

            var stopwatch = Stopwatch.StartNew();

            // magic for better performance
            var result = new Dictionary<string, MemoryStream>();
            if (styles.Count() > 4 && bytes.Length > 1 * 1024 * 1024)
            {
                Parallel.ForEach(styles, item =>
                {
                    result.Add(item.Name, ProccessStyle(bytes, item as ImageStyle));
                });
            }
            else
            {
                result = styles.ToDictionary(style => style.Name, style => ProccessStyle(bytes, style as ImageStyle));
            }

            stopwatch.Stop();

            Debug.WriteLine($"{stopwatch.Elapsed} for all styles");

            return result;
        }

        private MemoryStream ProccessStyle(byte[] bytes, ImageStyle style)
        {
            if (style == null)
            {
                return new MemoryStream(bytes, false);
            }

            var stopwatch = Stopwatch.StartNew();

            var outputStream = new MemoryStream();

            using (var image = new ImageSharp.Image(bytes, _configuration))
            {
                _imageTransformer.Transform(image, style);

                image.Save(outputStream);
            }

            stopwatch.Stop();

            Debug.WriteLine($"{stopwatch.Elapsed} for style {style.Name} {Thread.CurrentThread.ManagedThreadId}");

            return outputStream;
        }
    }
}
