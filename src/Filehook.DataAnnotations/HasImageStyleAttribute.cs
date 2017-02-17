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
            ImageResizeMode resizeMode = ImageResizeMode.PreserveAspectRatio) : base(name)
        {
            var resizeOptions = new ImageResizeOptions
            {
                Width = resizeWidth,
                Height = resizeHeight,
                Mode = resizeMode
            };

            Style = new ImageStyle(name, resizeOptions);
        }

        public HasImageStyleAttribute(string name) : base(name)
        {
            Style = new ImageStyle(name);
        }
    }
}
