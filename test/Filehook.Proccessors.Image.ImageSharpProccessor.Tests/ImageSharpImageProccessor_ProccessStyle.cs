using Filehook.Proccessors.Image.ImageSharpProccessor;
using Moq;
using System.IO;
using Xunit;

namespace Filehook.Proccessors.Image.Abstractions.Tests.ImageSharpProccessor
{
    public class ImageSharpImageProccessor_ProccessStyle
    {
        [Fact]
        public void EmptyOption_ReturnsEqualStream()
        {
            var mockStyleParser = new Mock<IImageTransformer>();

            mockStyleParser
                .Setup(x => x.Transform(It.IsAny<ImageSharp.Image>(), It.IsAny<ImageStyle>()));

            var mockImageStyleResolver = new Mock<IImageStyleResolver>();

            var imageSharpImageProccessor = new ImageSharpImageProccessor(mockStyleParser.Object, mockImageStyleResolver.Object);

            var bytes = TestFile.Create(TestImages.Jpeg.Lake).Bytes;

            var imageStyle = new ImageStyle("name");

            var result = imageSharpImageProccessor.ProccessStyle(bytes, imageStyle);

            Assert.NotNull(result);
            mockStyleParser.Verify(x => x.Transform(It.IsAny<ImageSharp.Image>(), It.Is<ImageStyle>(s => s == imageStyle)), Times.Once);
        }

        [Fact]
        public void NullOptions_ReturnsEqualStream()
        {
            var mockStyleParser = new Mock<IImageTransformer>();

            mockStyleParser
                .Setup(x => x.Transform(It.IsAny<ImageSharp.Image>(), It.Is<ImageStyle>(null)));

            var mockImageStyleResolver = new Mock<IImageStyleResolver>();

            var imageSharpImageProccessor = new ImageSharpImageProccessor(mockStyleParser.Object, mockImageStyleResolver.Object);

            var bytes = TestFile.Create(TestImages.Jpeg.Lake).Bytes;

            var result = imageSharpImageProccessor.ProccessStyle(bytes, null);

            Assert.NotNull(result);
            Assert.Equal(bytes, result.ToArray());
        }
    }
}
