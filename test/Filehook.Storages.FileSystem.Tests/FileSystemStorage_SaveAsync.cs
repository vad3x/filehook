using Filehook.Abstractions;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Options;
using System.IO;

namespace Filehook.Storages.FileSystem.Tests
{
    public class FileSystemStorage_SaveAsync
    {
        [Fact]
        public async Task WithEmptyFile_ReturnsSavedFileLocation()
        {
            const string relativeLocation = "./bin/helloworld.txt";
            var mockLocationTemplateParser = new Mock<ILocationTemplateParser>();

            mockLocationTemplateParser.Setup(x => x.SetBase(relativeLocation, It.IsAny<string>()))
                .Returns(relativeLocation);

            var locationTemplateParser = mockLocationTemplateParser.Object;

            var storageOptions = new FileSystemStorageOptions
            {
                BasePath = ""
            };

            var fileSystemStorage = new FileSystemStorage(Options.Create(storageOptions), locationTemplateParser);

            var stream = new MemoryStream();

            var location = await fileSystemStorage.SaveAsync(relativeLocation, stream);

            Assert.Equal(relativeLocation, location);
            Assert.True(File.Exists(location));
        }
    }
}
