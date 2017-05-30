using Filehook.Abstractions;
using Filehook.DataAnnotations.Tests.Fixtures;
using Microsoft.Extensions.Options;
using Xunit;

namespace Filehook.DataAnnotations.Tests
{
    public class AttributeFileStorageNameResolver_Resolve
    {
        [Fact]
        public void EntityWithDefinedStorage_ReturnsStorageName()
        {
            var options = new FileStorageNameResolverOptions
            {
                DefaultStorageName = "default"
            };

            var resolver = new AttributeFileStorageNameResolver(Options.Create(options));

            var result = resolver.Resolve<EntityWithDefinedStorage>((x) => x.Name);

            Assert.Equal("regular", result);
        }

        [Fact]
        public void EntityWithoutStorage_ReturnsDefault()
        {
            var options = new FileStorageNameResolverOptions
            {
                DefaultStorageName = "default"
            };

            var resolver = new AttributeFileStorageNameResolver(Options.Create(options));

            var result = resolver.Resolve<EntityWithoutStorage>((x) => x.Name);

            Assert.Equal(options.DefaultStorageName, result);
        }
    }
}
