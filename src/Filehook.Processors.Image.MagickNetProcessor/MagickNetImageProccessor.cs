using Filehook.Abstractions;
using Filehook.Proccessors.Image.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ImageMagick;

namespace Filehook.Proccessors.Image.MagickNetProccessor
{
    public class MagickNetImageProccessor : IFileProccessor
    {
        private readonly IImageTransformer _imageTransformer;

        private readonly ILogger _logger;

        public MagickNetImageProccessor(
            IImageTransformer imageTransformer,
            ILogger<MagickNetImageProccessor> logger)
        {
            _imageTransformer = imageTransformer ?? throw new ArgumentNullException(nameof(imageTransformer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool CanProccess(string fileExtension, byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return fileExtension == "jpg" || fileExtension == "jpeg";
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

            _logger.LogDebug($"processing started ...");

            var stopwatch = Stopwatch.StartNew();

            var result = new List<FileProccessingResult>();
            // magic for better performance
            //if (styles.Count() > 4 && bytes.Length > 1 * 1024 * 1024)
            //{
            //    Parallel.ForEach(styles, style =>
            //    {
            //        result.Add(ProccessStyle(bytes, style));
            //    });
            //}
            //else
            //{
                result = styles.Select(style => ProccessStyle(bytes, style)).ToList();
            //}

            stopwatch.Stop();

            _logger.LogDebug($"{stopwatch.Elapsed} for all styles");

            return Task.FromResult(result.AsEnumerable());
        }

        private FileProccessingResult ProccessStyle(byte[] bytes, FileStyle style)
        {
            var stopwatch = Stopwatch.StartNew();

            var outputStream = new MemoryStream();

            using (var image = new MagickImage(bytes))
            {
                var originalWidth = image.Width;
                var originalHeight = image.Height;

                var imageStyle = style as ImageStyle;
                if (imageStyle == null)
                {
                    outputStream = new MemoryStream(bytes, false);
                }
                else
                {
                    _imageTransformer.Transform(image, imageStyle);

                    //IEncoderOptions encoderOptions = null;
                    //if (image.CurrentImageFormat.GetType() == typeof(JpegFormat))
                    //{
                    //    encoderOptions = new JpegEncoderOptions
                    //    {
                    //        Quality = imageStyle.DecodeOptions.Quality,
                    //        Subsample = JpegSubsample.Ratio444
                    //    };
                    //}

                    // TODO switch
                    image.Format = MagickFormat.Pjpeg;
                    image.Quality = imageStyle.DecodeOptions.Quality;

                    image.Write(outputStream);
                }

                stopwatch.Stop();

                _logger.LogDebug($"{stopwatch.Elapsed} for style {style.Name}");

                _logger.LogInformation("Proccessed style '{0}' by '{1}'ms", style.Name, stopwatch.Elapsed.TotalMilliseconds);

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
}
