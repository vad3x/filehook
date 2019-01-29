using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Dawn;

using Filehook.Abstractions;

using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace Filehook.Analyzers.ImageSharp
{
    public class ImageSharpBlobAnalyzer : IBlobAnalyzer
    {
        private readonly ILogger<ImageSharpBlobAnalyzer> _logger;

        public ImageSharpBlobAnalyzer(ILogger<ImageSharpBlobAnalyzer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task AnalyzeAsync(Dictionary<string, string> metadata, FilehookFileInfo fileInfo)
        {
            Guard.Argument(metadata, nameof(metadata)).NotNull();
            Guard.Argument(fileInfo, nameof(fileInfo)).NotNull();

            Configuration configuration = Configuration.Default;

            IImageFormat format = configuration.ImageFormatsManager.FindFormatByMimeType(fileInfo.ContentType);
            if (format == null)
            {
                _logger.LogInformation(
                    "'{extenderName}' could not extend '{fileName}' of content-type: '{contentType}'",
                    nameof(ImageSharpBlobAnalyzer),
                    fileInfo.FileName,
                    fileInfo.ContentType);

                return Task.CompletedTask;
            }

            Stream stream = fileInfo.FileStream;
            stream.Position = 0;

            using (var originalImage = SixLabors.ImageSharp.Image.Load(Configuration.Default, stream, out var imageFormat))
            {
                TryAdd("width", originalImage.Width.ToString(), metadata);
                TryAdd("height", originalImage.Height.ToString(), metadata);
            }

            return Task.CompletedTask;
        }

        private void TryAdd(string key, string value, Dictionary<string, string> metadata)
        {
            if (!metadata.ContainsKey(key))
            {
                metadata.Add(key, value);
            }
            else
            {
                _logger.LogError($"Already contains '{key}'");
            }
        }
    }
}
