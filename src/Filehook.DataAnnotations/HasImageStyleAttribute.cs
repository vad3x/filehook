using Filehook.Proccessors.Image.Abstractions;

namespace Filehook.DataAnnotations
{
    public class HasImageStyleAttribute : HasFileStyleAttribute
    {
        /// <summary>
        /// Resize style
        /// </summary>
        /// <param name="name"> Style name </param>
        /// <param name="resizeWidth"> Max width (0 for auto) </param>
        /// <param name="resizeHeight"> Max height (0 for auto) </param>
        /// <param name="resizeMode"> Resize mode </param>
        public HasImageStyleAttribute(
            string name,
            int resizeWidth,
            int resizeHeight,
            ImageResizeMode resizeMode = ImageResizeMode.PreserveAspectRatio,
            Resampler resampler = Resampler.Auto,
            int quality = 82) : base(name)
        {
            var resizeOptions = new ImageResizeOptions
            {
                Width = resizeWidth,
                Height = resizeHeight,
                Mode = resizeMode
            };

            var decodeOptions = new ImageDecodeOptions
            {
                Quality = quality
            };

            Style = new ImageStyle(name, resizeOptions, decodeOptions);
        }

        public HasImageStyleAttribute(string name) : base(name)
        {
            Style = new ImageStyle(name, new ImageResizeOptions(), new ImageDecodeOptions());
        }
    }
}
