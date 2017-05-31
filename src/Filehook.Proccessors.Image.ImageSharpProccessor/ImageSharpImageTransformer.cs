using Filehook.Proccessors.Image.Abstractions;
using ImageSharp;
using ImageSharp.Processing;
using System;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public class ImageSharpImageTransformer : IImageTransformer
    {
        public void Transform(Image<Rgba32> image, ImageStyle style)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            if (style.ResizeOptions != null)
            {
                image.Resize(ToResizeOptions(image, style.ResizeOptions));
            }

            // TODO other
        }

        private ResizeOptions ToResizeOptions(Image<Rgba32> image, ImageResizeOptions resizeOptions)
        {
            ResizeMode mode;
            switch (resizeOptions.Mode)
            {
                case ImageResizeMode.PreserveAspectRatio: mode = ResizeMode.Max; break;
                case ImageResizeMode.IgnoreAspectRatio: mode = ResizeMode.Stretch; break;
                case ImageResizeMode.ShrinkLarger: mode = ResizeMode.Min; break;
                case ImageResizeMode.Fill: mode = ResizeMode.Min; break;
                default: throw new NotImplementedException();
            }

            var resizeOptionsResult = new ResizeOptions
            {
                Size = new Size(resizeOptions.Width, resizeOptions.Height),
                Mode = mode
            };

            switch (resizeOptions.Resampler)
            {
                // https://www.imagemagick.org/discourse-server/viewtopic.php?t=15742
                case Resampler.Auto:
                    {
                        if (image.Width < resizeOptions.Width)
                        {
                            resizeOptionsResult.Sampler = new MitchellNetravaliResampler();
                            break;
                        }

                        // TODO split
                        resizeOptionsResult.Sampler = new Lanczos2Resampler();
                        break;
                    }
                case Resampler.Bicubic:
                    resizeOptionsResult.Sampler = new BicubicResampler();
                    break;
                case Resampler.Box:
                    resizeOptionsResult.Sampler = new BoxResampler();
                    break;
                case Resampler.CatmullRom:
                    resizeOptionsResult.Sampler = new CatmullRomResampler();
                    break;
                case Resampler.Hermite:
                    resizeOptionsResult.Sampler = new HermiteResampler();
                    break;
                case Resampler.MitchellNetravali:
                    resizeOptionsResult.Sampler = new MitchellNetravaliResampler();
                    break;
                case Resampler.NearestNeighbor:
                    resizeOptionsResult.Sampler = new NearestNeighborResampler();
                    break;
                case Resampler.Robidoux:
                    resizeOptionsResult.Sampler = new RobidouxResampler();
                    break;
                case Resampler.RobidouxSharp:
                    resizeOptionsResult.Sampler = new RobidouxSharpResampler();
                    break;
                case Resampler.Spline:
                    resizeOptionsResult.Sampler = new SplineResampler();
                    break;
                case Resampler.Lanczos2:
                    resizeOptionsResult.Sampler = new Lanczos2Resampler();
                    break;
                case Resampler.Lanczos3:
                    resizeOptionsResult.Sampler = new Lanczos3Resampler();
                    break;
                case Resampler.Lanczos5:
                    resizeOptionsResult.Sampler = new Lanczos5Resampler();
                    break;
                case Resampler.Lanczos8:
                    resizeOptionsResult.Sampler = new Lanczos8Resampler();
                    break;
                case Resampler.Triangle:
                    resizeOptionsResult.Sampler = new TriangleResampler();
                    break;
                case Resampler.Welch:
                    resizeOptionsResult.Sampler = new WelchResampler();
                    break;
                default:
                    throw new NotImplementedException($"'{resizeOptions.Resampler}' Resampler is not implemented");
            }

            return resizeOptionsResult;
        }
    }
}
