using System;
using System.Linq;
using Filehook.Abstractions;
using Filehook.DataAnnotations.Tests.Fixtures;
using Xunit;

namespace Filehook.DataAnnotations.Tests
{
    public class AttributeFileStyleResolver_Resolve
    {
        [Fact]
        public void EntityWithOnePropertyStyle_ReturnsTwoStyles()
        {
            var resolver = new AttributeFileStyleResolver();

            var result = resolver.Resolve<EntityWithOnePropertyStyle>((x) => x.FileName);

            Assert.Equal(2, result.Count());
            Assert.Equal(result.First().Name, "regular");
            Assert.Equal(result.Last().Name, FilehookConsts.OriginalStyleName);
        }

        [Fact]
        public void EntityWithDuplicatingStyle_ReturnsArgumentException()
        {
            var resolver = new AttributeFileStyleResolver();

            Assert.Throws(typeof(ArgumentException), () => resolver.Resolve<EntityWithDuplicatingStyleName>((x) => x.FileName));
        }
    }
}
