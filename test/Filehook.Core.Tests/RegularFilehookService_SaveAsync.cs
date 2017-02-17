using Filehook.Abstractions;
using Filehook.Core.Tests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Filehook.Core.Tests
{
    public class RegularFilehookService_SaveAsync
    {
        [Fact]
        public void Ok()
        {
            var storageName = "storageName";
            var style = "some_style";
            var someUrl = "someUrl";

            var data = new byte[] { 0xFF };

            var mockFileStorageNameResolver = new Mock<IFileStorageNameResolver>();
            var mockFileStyleResolver = new Mock<IFileStyleResolver>();
            var mockFileStorage = new Mock<IFileStorage>();
            var mockFileProccessor = new Mock<IFileProccessor>();
            var mockLocationTemplateParser = new Mock<ILocationTemplateParser>();
            var mockLocationParamFormatter = new Mock<ILocationParamFormatter>();

            mockFileStorageNameResolver
                .Setup(x => x.Resolve(It.IsAny<Expression<Func<EntityWithoutStorage, string>>>()))
                .Returns(storageName);

            mockFileStyleResolver
                .Setup(x => x.Resolve(It.IsAny<Expression<Func<EntityWithoutStorage, string>>>()))
                .Returns(new[] { new FileStyle(style) });

            mockFileStorage
                .SetupGet(x => x.Name)
                .Returns(storageName);

            mockFileStorage
                .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<MemoryStream>()))
                .Returns(Task.FromResult("path"));

            mockFileStorage
                .Setup(x => x.GetUrl(It.IsAny<string>()))
                .Returns(someUrl);

            mockFileProccessor
                .Setup(x => x.CanProccess(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Returns(true);

            mockFileProccessor
                .Setup(x => x.Proccess(It.IsAny<byte[]>(), It.IsAny<IEnumerable<FileStyle>>()))
                .Returns(new Dictionary<string, MemoryStream> { { style, new MemoryStream(data, false) } });

            mockLocationTemplateParser
              .Setup(x => x.Parse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .Returns("parsedTemplate");

            mockLocationParamFormatter
                .Setup(x => x.Format(It.IsAny<string>()))
                .Returns("formattedValue");

            var regularFilehookService = new RegularFilehookService(
                mockFileStorageNameResolver.Object,
                mockFileStyleResolver.Object,
                new[] { mockFileStorage.Object },
                new[] { mockFileProccessor.Object },
                mockLocationTemplateParser.Object,
                mockLocationParamFormatter.Object);

            var entity = new EntityWithoutStorage
            {
                Name = "name1.ext"
            };

            var result = regularFilehookService.SaveAsync(entity, e => e.Name, data, 1.ToString());

            // TODO verify
        }
    }
}
