using Filehook.Abstractions;

namespace Filehook.Proccessors.Image.Abstractions
{
    public class ImageStyle : IFileStyle
    {
        public ImageStyle(string name, ImageResizeOptions resizeOptions = null)
        {
            Name = name;

            ResizeOptions = resizeOptions;
        }

        public string Name { get; set; }

        public ImageResizeOptions ResizeOptions { get; set; }
    }
}
