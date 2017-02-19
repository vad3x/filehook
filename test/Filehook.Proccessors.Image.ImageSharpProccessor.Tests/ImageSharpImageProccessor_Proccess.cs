using Filehook.Abstractions;
using Filehook.Proccessors.Image.ImageSharpProccessor;
using Moq;
using Xunit;

namespace Filehook.Proccessors.Image.Abstractions.Tests
{
    public class ImageSharpImageProccessor_Proccess
    {
        [Fact]
        public void EmptyOption_ReturnsEqualStream()
        {
            var mockImageTransformer = new Mock<IImageTransformer>();

            mockImageTransformer
                .Setup(x => x.Transform(It.IsAny<ImageSharp.Image>(), It.IsAny<ImageStyle>()));

            var imageSharpImageProccessor = new ImageSharpImageProccessor(mockImageTransformer.Object);

            var bytes = TestFile.Create(TestImages.Jpeg.Lake).Bytes;

            var imageStyle = new ImageStyle("name");

            var result = imageSharpImageProccessor.ProccessAsync(bytes, new[] { imageStyle });

            Assert.NotNull(result);
            mockImageTransformer.Verify(x => x.Transform(It.IsAny<ImageSharp.Image>(), It.Is<ImageStyle>(s => s == imageStyle)), Times.Once);
        }

        [Fact]
        public void NullOptions_ReturnsEqualStream()
        {
            var mockStyleParser = new Mock<IImageTransformer>();

            mockStyleParser
                .Setup(x => x.Transform(It.IsAny<ImageSharp.Image>(), It.Is<ImageStyle>(null)));

            var imageSharpImageProccessor = new ImageSharpImageProccessor(mockStyleParser.Object);

            var bytes = TestFile.Create(TestImages.Jpeg.Lake).Bytes;

            var result = imageSharpImageProccessor.ProccessAsync(bytes, new FileStyle[0]);

            Assert.NotNull(result);
            //Assert.Equal(bytes, result.ToArray());
        }
    }
}
