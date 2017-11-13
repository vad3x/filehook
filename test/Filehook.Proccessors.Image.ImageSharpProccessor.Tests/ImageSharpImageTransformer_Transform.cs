using Filehook.Proccessors.Image.ImageSharpProccessor;
using SixLabors.ImageSharp;
using Xunit;

namespace Filehook.Proccessors.Image.Abstractions.Tests.ImageSharpProccessor
{
    public class ImageSharpImageTransformer_Transform
    {
        [Fact]
        public void WithFillResizeOptions_Ok()
        {
            var image = Transform(100, 100, ImageResizeMode.Fill);

            Assert.Equal(150, image.Width);
            Assert.Equal(100, image.Height);
        }

        [Fact]
        public void WithIgnoreAspectRatioResizeOptions_Ok()
        {
            var image = Transform(100, 100, ImageResizeMode.IgnoreAspectRatio);

            Assert.Equal(100, image.Width);
            Assert.Equal(100, image.Height);
        }

        [Fact]
        public void WithPreserveAspectRatioSmallerResizeOptions_Ok()
        {
            var image = Transform(100, 100, ImageResizeMode.PreserveAspectRatio);

            Assert.Equal(100, image.Width);
            Assert.Equal(67, image.Height);
        }

        [Fact]
        public void WithPreserveAspectRatioLargerResizeOptions_Ok()
        {
            var image = Transform(3000, 3000, ImageResizeMode.PreserveAspectRatio);

            Assert.Equal(3000, image.Width);
            Assert.Equal(1999, image.Height);
        }

        [Fact]
        public void WithShrinkLargerResizeOptions_Ok()
        {
            var image = Transform(10000, 10000, ImageResizeMode.ShrinkLarger);

            Assert.Equal(1280, image.Width);
            Assert.Equal(853, image.Height);
        }

        private Image<Rgba32> Transform(int width, int height, ImageResizeMode mode)
        {
            var transformer = new ImageSharpImageTransformer();

            var imageStyle = new ImageStyle("test")
            {
                ResizeOptions = new ImageResizeOptions
                {
                    Width = width,
                    Height = height,
                    Mode = mode
                }
            };

            var configuration = Configuration.Default;

            var bytes = TestFile.Create(TestImages.Jpeg.Lake).Bytes;

            var image = SixLabors.ImageSharp.Image.Load(configuration, bytes);

            transformer.Transform(image, imageStyle);

            return image;
        }
    }
}
