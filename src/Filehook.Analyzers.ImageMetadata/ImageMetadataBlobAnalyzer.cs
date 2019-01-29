using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dawn;

using Filehook.Abstractions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Filehook.Analyzers.ImageMetadata
{
    public class ImageMetadataBlobAnalyzer : IBlobAnalyzer
    {
        private readonly ILogger<ImageMetadataBlobAnalyzer> _logger;
        private readonly ImageMetadataBlobAnalyzerOptions _options;

        public ImageMetadataBlobAnalyzer(ILogger<ImageMetadataBlobAnalyzer> logger, IOptions<ImageMetadataBlobAnalyzerOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public Task AnalyzeAsync(Dictionary<string, string> metadata, FilehookFileInfo fileInfo)
        {
            Guard.Argument(metadata, nameof(metadata)).NotNull();
            Guard.Argument(fileInfo, nameof(fileInfo)).NotNull();

            Stream stream = fileInfo.FileStream;
            stream.Position = 0;

            Size size;
            try
            {
                size = GetDimensions(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not analyze file: '{fileName}' baceuse of exception: '{exception}'", fileInfo.FileName, ex.Message);

                return Task.CompletedTask;
            }

            TryAdd("width", size.Width.ToString(), metadata);
            TryAdd("height", size.Height.ToString(), metadata);

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

        /// <summary>
        /// Gets the dimensions of an image.
        /// </summary>
        /// <param name="path">The path of the image to get the dimensions of.</param>
        /// <returns>The dimensions of the specified image.</returns>
        /// <exception cref="ArgumentException">The image was of an unrecognized format.</exception>
        private Size GetDimensions(Stream stream)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.Default, true))
            {
                return GetDimensions(binaryReader);
            }
        }

        /// <summary>
        /// Gets the dimensions of an image.
        /// </summary>
        /// <param name="path">The path of the image to get the dimensions of.</param>
        /// <returns>The dimensions of the specified image.</returns>
        /// <exception cref="ArgumentException">The image was of an unrecognized format.</exception>
        private Size GetDimensions(BinaryReader binaryReader)
        {
            int maxMagicBytesLength = _options.ImageFormatDecoders.Keys.OrderByDescending(x => x.Length).First().Length;

            byte[] magicBytes = new byte[maxMagicBytesLength];

            for (int i = 0; i < maxMagicBytesLength; i++)
            {
                magicBytes[i] = binaryReader.ReadByte();

                foreach (var kvPair in _options.ImageFormatDecoders)
                {
                    if (StartsWith(magicBytes, kvPair.Key))
                    {
                        return kvPair.Value(binaryReader);
                    }
                }
            }

            throw new NotImplementedException("Unknown format");
        }

        private bool StartsWith(byte[] thisBytes, byte[] thatBytes)
        {
            for (int i = 0; i < thatBytes.Length; i++)
            {
                if (thisBytes[i] != thatBytes[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
