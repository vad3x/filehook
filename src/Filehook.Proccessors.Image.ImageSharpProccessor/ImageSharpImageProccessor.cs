using Filehook.Abstractions;
using Filehook.Proccessors.Image.Abstractions;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public class ImageSharpImageProccessor : IFileProccessor
    {
        private readonly IImageTransformer _imageTransformer;

        private readonly Configuration _configuration;

        private readonly ILogger _logger;

        public ImageSharpImageProccessor(
            IImageTransformer imageTransformer,
            ILogger<ImageSharpImageProccessor> logger)
        {
            _imageTransformer = imageTransformer ?? throw new ArgumentNullException(nameof(imageTransformer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _configuration = Configuration.Default;
        }

        public bool CanProccess(string fileExtension, byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return _configuration.FindFormatByFileExtension(fileExtension) != null;
        }

        public Task<IEnumerable<FileProccessingResult>> ProccessAsync(byte[] bytes, IEnumerable<FileStyle> styles)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (styles == null)
            {
                throw new ArgumentNullException(nameof(styles));
            }

            _logger.LogInformation($"processing started ...");

            var result = new List<FileProccessingResult>();

            var stopwatch = Stopwatch.StartNew();

            using (var image = SixLabors.ImageSharp.Image.Load(_configuration, bytes, out var imageFormat))
            {
                stopwatch.Stop();
                _logger.LogInformation("Loaded '{0}' in '{1}'ms", imageFormat.MimeTypes, stopwatch.Elapsed.TotalMilliseconds);
                stopwatch.Restart();

                result = styles.Select(style => ProccessStyle(bytes, image, imageFormat, style)).ToList();
            }

            stopwatch.Stop();

            _logger.LogDebug($"{stopwatch.Elapsed.TotalMilliseconds} for all styles");

            return Task.FromResult(result.AsEnumerable());
        }

        private FileProccessingResult ProccessStyle(
            byte[] bytes,
            Image<Rgba32> image,
            IImageFormat imageFormat,
            FileStyle style)
        {
            var outputStream = new MemoryStream();

            var originalWidth = image.Width;
            var originalHeight = image.Height;

            var imageStyle = style as ImageStyle;
            if (imageStyle == null)
            {
                outputStream = new MemoryStream(bytes, false);
            }
            else
            {
                var stopwatch = Stopwatch.StartNew();

                _imageTransformer.Transform(image, imageStyle);

                IImageEncoder imageEncoder = null;
                if (imageFormat.Name == ImageFormats.Jpeg.Name) // TODO other formats
                {
                    imageEncoder = new JpegEncoder
                    {
                        Quality = imageStyle.DecodeOptions.Quality,
                        Subsample = JpegSubsample.Ratio444
                    };
                }

                stopwatch.Stop();
                _logger.LogInformation("Transformed '{0}' in '{1}'ms", style.Name, stopwatch.Elapsed.TotalMilliseconds);
                stopwatch.Restart();

                image.Save(outputStream, imageEncoder);

                stopwatch.Stop();
                _logger.LogInformation("Saved '{0}' in '{1}'ms", style.Name, stopwatch.Elapsed.TotalMilliseconds);
            }

            return new FileProccessingResult
            {
                Style = style,
                Stream = outputStream,
                Meta = new ImageProccessingResultMeta
                {
                    OriginalWidth = originalWidth,
                    OriginalHeight = originalHeight,
                    Width = image.Width,
                    Height = image.Height
                }
            };
        }
    }
}
