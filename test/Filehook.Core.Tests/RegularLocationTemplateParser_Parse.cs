using Microsoft.Extensions.Options;
using Xunit;

namespace Filehook.Core.Tests
{
    public class RegularLocationTemplateParser_Parse
    {
        [Theory]
        [InlineData(
            ":base/public/:class/:attachmentName/:attachmentId/:style/:filename",
            "Klass",
            "Name",
            "Id",
            "SomeStyle",
            "FileName",
            ":base/public/Klass/Name/Id/SomeStyle/FileName")]
        [InlineData(
            ":base/public/:class/:attachmentName/:attachmentId/:style/:filename",
            null,
            "Name",
            "Id",
            "SomeStyle",
            "FileName",
            ":base/public/:class/Name/Id/SomeStyle/FileName")]
        public void WithSpecificLocationTemplate_ShouldReturnCorrectString(
            string locationTemplate,
            string className,
            string attachmentName,
            string attachmentId,
            string style,
            string filename,
            string expected)
        {
            var options = new RegularLocationTemplateParserOptions();

            var formatter = new RegularLocationTemplateParser(Options.Create(options));

            var result = formatter.Parse(
                    className: className,
                    attachmentName: attachmentName,
                    attachmentId: attachmentId,
                    style: style,
                    filename: filename,
                    locationTemplate: locationTemplate);

            Assert.Equal(expected, result);
        }
    }
}
