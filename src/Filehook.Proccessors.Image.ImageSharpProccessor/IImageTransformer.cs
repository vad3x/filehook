using Filehook.Proccessors.Image.Abstractions;
using SixLabors.ImageSharp.PixelFormats;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public interface IImageTransformer
    {
        void Transform(SixLabors.ImageSharp.Image<Rgba32> image, ImageStyle style);
    }
}
