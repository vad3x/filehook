using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using Filehook.Abstractions;
using Filehook.Proccessors.Image.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public class ImageSharpImageProccessor : IFileProccessor
    {
        private readonly IImageTransformer _imageTransformer;

        private readonly Configuration _configuration;

        private readonly ILogger _logger;

        private readonly ImageSharpImageProccessorOptions _options;

        public ImageSharpImageProccessor(
            IImageTransformer imageTransformer,
            IOptions<ImageSharpImageProccessorOptions> options,
            ILogger<ImageSharpImageProccessor> logger)
        {
            _imageTransformer = imageTransformer ?? throw new ArgumentNullException(nameof(imageTransformer));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
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

            Stopwatch stopwatch = null;
            if (_logger.IsEnabled(LogLevel.Information))
            {
                stopwatch = Stopwatch.StartNew();
            }

            using (var originalImage = SixLabors.ImageSharp.Image.Load(_configuration, bytes, out var imageFormat))
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    stopwatch.Stop();
                    _logger.LogInformation("Loaded '{imageFormatName}' in '{elapsed}'ms", imageFormat.Name, stopwatch.Elapsed.TotalMilliseconds);
                    stopwatch.Start();
                }

                Parallel.ForEach(styles, _options.ParallelOptions, style =>
                {
                    result.Add(ProccessStyle(bytes, originalImage, imageFormat, style));
                });

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    stopwatch.Stop();
                    _logger.LogInformation("Processed '{imageFormatName}' in '{elapsed}'ms", imageFormat.Name, stopwatch.Elapsed.TotalMilliseconds);
                }
            }

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();

            return Task.FromResult(result.AsEnumerable());
        }

        private FileProccessingResult ProccessStyle(
            byte[] bytes,
            Image<Rgba32> originalImage,
            IImageFormat imageFormat,
            FileStyle style)
        {
            Stopwatch stopwatch = null;
            if (_logger.IsEnabled(LogLevel.Information))
            {
                stopwatch = Stopwatch.StartNew();
            }

            var outputStream = new MemoryStream();

            var originalWidth = originalImage.Width;
            var originalHeight = originalImage.Height;

            int width;
            int height;

            var imageStyle = style as ImageStyle;
            if (imageStyle == null)
            {
                width = originalImage.Width;
                height = originalImage.Height;

                outputStream = new MemoryStream(bytes, false);
            }
            else
            {
                using (var image = originalImage.Clone())
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        stopwatch.Stop();
                        _logger.LogInformation("Cloned '{styleName}' in '{elapsed}'ms", style.Name, stopwatch.Elapsed.TotalMilliseconds);
                        stopwatch.Restart();
                    }

                    _imageTransformer.Transform(image, imageStyle);

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        stopwatch.Stop();
                        _logger.LogInformation("Transformed '{styleName}' in '{elapsed}'ms", style.Name, stopwatch.Elapsed.TotalMilliseconds);
                        stopwatch.Restart();
                    }

                    image.Save(outputStream, GetEncoder(imageFormat, imageStyle));

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        stopwatch.Stop();
                        _logger.LogInformation("Saved '{styleName}' in '{elapsed}'ms", style.Name, stopwatch.Elapsed.TotalMilliseconds);
                    }

                    width = image.Width;
                    height = image.Height;
                }
            }

            return new FileProccessingResult
            {
                Style = style,
                Stream = outputStream,
                Meta = new ImageProccessingResultMeta
                {
                    OriginalWidth = originalWidth,
                    OriginalHeight = originalHeight,
                    Width = width,
                    Height = height
                }
            };
        }

        private IImageEncoder GetEncoder(IImageFormat imageFormat, ImageStyle imageStyle)
        {
            if (imageFormat.Name == ImageFormats.Jpeg.Name) // TODO other formats
            {
                return new JpegEncoder
                {
                    Quality = imageStyle.EncodeOptions.Quality,
                    Subsample = JpegSubsample.Ratio444
                };
            }

            throw new NotImplementedException($"No Encoder configured for '{imageStyle.EncodeOptions.MimeType}' MimeType");
        }
    }
}
