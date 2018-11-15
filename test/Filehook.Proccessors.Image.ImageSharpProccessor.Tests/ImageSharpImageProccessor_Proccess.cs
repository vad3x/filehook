using Filehook.Abstractions;
using Filehook.Proccessors.Image.ImageSharpProccessor;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Xunit;

namespace Filehook.Proccessors.Image.Abstractions.Tests
{
    public class ImageSharpImageProccessor_Proccess
    {
        [Fact]
        public void EmptyOption_ReturnsEqualStream()
        {
            var mockImageTransformer = new Mock<IImageTransformer>();
            var mockOptions = new Mock<IOptions<ImageSharpImageProccessorOptions>>();
            var mockLogger = new Mock<ILogger<ImageSharpImageProccessor>>();

            mockImageTransformer
                .Setup(x => x.Transform(It.IsAny<Image<Rgba32>>(), It.IsAny<ImageStyle>()));

            var imageSharpImageProccessor = new ImageSharpImageProccessor(mockImageTransformer.Object, mockOptions.Object, mockLogger.Object);

            var bytes = TestFile.Create(TestImages.Jpeg.Lake).Bytes;

            var imageStyle = new ImageStyle("name");

            var result = imageSharpImageProccessor.ProccessAsync(bytes, new[] { imageStyle });

            Assert.NotNull(result);
            mockImageTransformer.Verify(x => x.Transform(It.IsAny<Image<Rgba32>>(), It.Is<ImageStyle>(s => s == imageStyle)), Times.Once);
        }

        [Fact]
        public void NullOptions_ReturnsEqualStream()
        {
            var mockStyleParser = new Mock<IImageTransformer>();
            var mockOptions = new Mock<IOptions<ImageSharpImageProccessorOptions>>();
            var mockLogger = new Mock<ILogger<ImageSharpImageProccessor>>();

            mockStyleParser
                .Setup(x => x.Transform(It.IsAny<Image<Rgba32>>(), It.Is<ImageStyle>(null)));

            var imageSharpImageProccessor = new ImageSharpImageProccessor(mockStyleParser.Object, mockOptions.Object, mockLogger.Object);

            var bytes = TestFile.Create(TestImages.Jpeg.Lake).Bytes;

            var result = imageSharpImageProccessor.ProccessAsync(bytes, new FileStyle[0]);

            Assert.NotNull(result);
        }
    }
}
