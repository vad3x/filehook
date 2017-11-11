using Filehook.Abstractions;
using Filehook.Core.Tests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;

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
            var mockParamNameResolver = new Mock<IParamNameResolver>();
            var mockEntityIdResolver = new Mock<IEntityIdResolver>();

            var options = new FilehookOptions();

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

            // TODO
            // var fileProccessorResult = new IEnumerable<FileProccessingResult> { { style, new MemoryStream(data, false) } };
            // mockFileProccessor
            //     .Setup(x => x.ProccessAsync(It.IsAny<byte[]>(), It.IsAny<IEnumerable<FileStyle>>()))
            //     .Returns(Task.FromResult(fileProccessorResult));

            mockLocationTemplateParser
              .Setup(x => x.Parse(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
              .Returns("parsedTemplate");

            mockLocationParamFormatter
                .Setup(x => x.Format(It.IsAny<string>()))
                .Returns("formattedValue");

            var regularFilehookService = new RegularFilehookService(
                Options.Create(options),
                mockFileStorageNameResolver.Object,
                mockFileStyleResolver.Object,
                new[] { mockFileStorage.Object },
                new[] { mockFileProccessor.Object },
                mockLocationTemplateParser.Object,
                mockLocationParamFormatter.Object,
                mockParamNameResolver.Object,
                mockEntityIdResolver.Object);

            var entity = new EntityWithoutStorage
            {
                Id = "1",
                Name = "name1.ext"
            };

            var result = regularFilehookService.SaveAsync(entity, e => e.Name, data);

            // TODO verify
        }
    }
}
