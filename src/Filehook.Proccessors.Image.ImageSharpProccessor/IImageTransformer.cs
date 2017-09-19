using Filehook.Proccessors.Image.Abstractions;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public interface IImageTransformer
    {
        void Transform(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.Rgba32> image, ImageStyle style);
    }
}
