using Filehook.Abstractions;

namespace Filehook.Proccessors.Image.Abstractions
{
    public class ImageProccessingResult : FileProccessingResult
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }
}