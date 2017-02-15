using Filehook.Proccessors.Image.Abstractions;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public interface IImageTransformer
    {
        void Transform(ImageSharp.Image image, ImageStyle style);
    }
}
