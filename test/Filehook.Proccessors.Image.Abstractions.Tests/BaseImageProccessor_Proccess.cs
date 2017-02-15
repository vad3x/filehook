using Moq;
using System.IO;
using System.Linq;
using Xunit;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Filehook.Proccessors.Image.Abstractions.Tests.Fixtures;

namespace Filehook.Proccessors.Image.Abstractions.Tests
{
    public class BaseImageProccessor_Proccess
    {
        [Fact]
        public void WithEntityWithOnePropertyStyle_ReturnsOneStream()
        {
            var imageStyle = new ImageStyle("stylename");
            var bytes = new byte[10];

            var mockImageStyleResolver = new Mock<IImageStyleResolver>();

            mockImageStyleResolver
                .Setup(x => x.Resolve(It.IsAny<Expression<Func<EntityWithOnePropertyStyle, string>>>()))
                .Returns(new List<ImageStyle>() { imageStyle });

            var mockBaseImageProccessor = new Mock<BaseImageProccessor>(mockImageStyleResolver.Object);
            
            mockBaseImageProccessor
                .Setup(x => x.ProccessStyle(It.Is<byte[]> (s => s == bytes), It.Is<ImageStyle>(s => s == imageStyle)))
                .Returns(new MemoryStream(bytes));

            var baseImageProccessor = mockBaseImageProccessor.Object;

            var fixtureEntity = new EntityWithOnePropertyStyle
            {
                ImageName = "image_name.any"
            };

            var result = baseImageProccessor.Proccess(fixtureEntity, x => x.ImageName, bytes);

            Assert.Equal(1, result.Count);
            Assert.Equal("stylename", result.First().Key);
            Assert.Equal(bytes, result.First().Value.ToArray());

            mockBaseImageProccessor.Verify();
        }
    }
}
