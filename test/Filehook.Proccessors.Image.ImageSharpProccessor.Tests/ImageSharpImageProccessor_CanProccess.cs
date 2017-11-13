using Filehook.Proccessors.Image.ImageSharpProccessor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Filehook.Proccessors.Image.Abstractions.Tests.ImageSharpProccessor
{
    public class ImageSharpImageProccessor_CanProccess
    {
        [Theory]
        [InlineData("jpg", new byte[] { 0xFF, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8 })]
        [InlineData("jpeg", new byte[] { 0xFF, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8, 0xD8 })]
        public void ShouldReturnTrue(string fileExtension, byte[] bytes)
        {
            var mockImageTransfomer = new Mock<IImageTransformer>();
            var mockOptions = new Mock<IOptions<ImageSharpImageProccessorOptions>>();
            var mockLogger = new Mock<ILogger<ImageSharpImageProccessor>>();

            var imageSharpImageProccessor = new ImageSharpImageProccessor(mockImageTransfomer.Object, mockOptions.Object, mockLogger.Object);

            var result = imageSharpImageProccessor.CanProccess(fileExtension, bytes);

            Assert.True(result);
        }

        [Theory]
        [InlineData("zip", new byte[] { 0xFF, 0x45 })]
        [InlineData("pdf", new byte[] { 0xFF, 0x45 })]
        [InlineData("any", new byte[] { 0xFF, 0x45 })]
        public void ShouldReturnFalse(string fileExtension, byte[] bytes)
        {
            var mockStyleParser = new Mock<IImageTransformer>();
            var mockOptions = new Mock<IOptions<ImageSharpImageProccessorOptions>>();
            var mockLogger = new Mock<ILogger<ImageSharpImageProccessor>>();

            var imageSharpImageProccessor = new ImageSharpImageProccessor(mockStyleParser.Object, mockOptions.Object, mockLogger.Object);

            var result = imageSharpImageProccessor.CanProccess(fileExtension, bytes);

            Assert.False(result);
        }
    }
}
