using Filehook.Abstractions;
using Filehook.Proccessors.Image.Abstractions.Tests.Fixtures;
using Filehook.Proccessors.Image.ImageSharpProccessor;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Filehook.Proccessors.Image.Abstractions.Tests
{
    public class ImageSharpImageProccessor_Proccess
    {
        //[Fact]
        //public void WithEntityWithOnePropertyStyle_ReturnsOneStream()
        //{
        //    var imageStyle = new ImageStyle("stylename");
        //    var bytes = new byte[10];

        //    var mockImageTransformer = new Mock<IImageTransformer>();

        //    mockImageTransformer
        //        .Setup(x => x.Transform(It.IsAny<ImageSharp.Image>(), It.IsAny<ImageStyle>()));

        //    mockImageStyleResolver
        //        .Setup(x => x.Resolve(It.IsAny<Expression<Func<EntityWithOnePropertyStyle, string>>>()))
        //        .Returns(new List<ImageStyle>() { imageStyle });

        //    var mockBaseImageProccessor = new Mock<BaseImageProccessor>(mockImageStyleResolver.Object);
            
        //    mockBaseImageProccessor
        //        .Setup(x => x.ProccessStyle(It.Is<byte[]> (s => s == bytes), It.Is<ImageStyle>(s => s == imageStyle)))
        //        .Returns(new MemoryStream(bytes));

        //    var baseImageProccessor = new ImageSharpImageProccessor();

        //    var fixtureEntity = new EntityWithOnePropertyStyle
        //    {
        //        ImageName = "image_name.any"
        //    };

        //    var result = baseImageProccessor.Proccess(fixtureEntity, x => x.ImageName, bytes);

        //    Assert.Equal(1, result.Count);
        //    Assert.Equal("stylename", result.First().Key);
        //    Assert.Equal(bytes, result.First().Value.ToArray());

        //    mockBaseImageProccessor.Verify();
        //}

        [Fact]
        public void EmptyOption_ReturnsEqualStream()
        {
            var mockImageTransformer = new Mock<IImageTransformer>();

            mockImageTransformer
                .Setup(x => x.Transform(It.IsAny<ImageSharp.Image>(), It.IsAny<ImageStyle>()));

            var imageSharpImageProccessor = new ImageSharpImageProccessor(mockImageTransformer.Object);

            var bytes = TestFile.Create(TestImages.Jpeg.Lake).Bytes;

            var imageStyle = new ImageStyle("name");

            var result = imageSharpImageProccessor.Proccess(bytes, new[] { imageStyle });

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

            var result = imageSharpImageProccessor.Proccess(bytes, new FileStyle[0]);

            Assert.NotNull(result);
            //Assert.Equal(bytes, result.ToArray());
        }
    }
}
