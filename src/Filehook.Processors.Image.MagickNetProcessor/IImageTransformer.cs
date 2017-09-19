using Filehook.Proccessors.Image.Abstractions;
using ImageMagick;

namespace Filehook.Proccessors.Image.MagickNetProccessor
{
    public interface IImageTransformer
    {
        void Transform(MagickImage image, ImageStyle style);
    }
}
