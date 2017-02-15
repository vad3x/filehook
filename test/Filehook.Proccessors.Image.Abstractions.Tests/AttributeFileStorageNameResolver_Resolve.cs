using System;
using System.Linq;
using Filehook.Core;
using Filehook.Proccessors.Image.Abstractions.Tests.Fixtures;
using Xunit;

namespace Filehook.Proccessors.Image.Abstractions.Tests
{
    public class AttributeImageStyleResolver_Resolve
    {
        [Fact]
        public void EntityWithOnePropertyStyle_ReturnsTwoStyles()
        {
            var resolver = new AttributeImageStyleResolver();

            var result = resolver.Resolve<EntityWithOnePropertyStyle>((x) => x.ImageName);

            Assert.Equal(2, result.Count());
            Assert.Equal(result.First().Name, "regular");
            Assert.Equal(result.Last().Name, FilehookConsts.OriginalStyleName);
        }

        [Fact]
        public void EntityWithDuplicatingStyle_ReturnsArgumentException()
        {
            var resolver = new AttributeImageStyleResolver();

            Assert.Throws(typeof(ArgumentException), () => resolver.Resolve<EntityWithDuplicatingStyleName>((x) => x.ImageName));
        }
    }
}
