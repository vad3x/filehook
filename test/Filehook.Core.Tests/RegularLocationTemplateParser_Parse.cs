using Microsoft.Extensions.Options;
using Xunit;

namespace Filehook.Core.Tests
{
    public class RegularLocationTemplateParser_Parse
    {
        [Theory]
        [InlineData(
            ":base/public/:objectClass/:propertyName/:objectId/:style/:filename",
            "Klass",
            "Name",
            "Id",
            "SomeStyle",
            "FileName",
            ":base/public/Klass/Name/Id/SomeStyle/FileName")]
        [InlineData(
            ":base/public/:objectClass/:propertyName/:objectId/:style/:filename",
            null,
            "Name",
            "Id",
            "SomeStyle",
            "FileName",
            ":base/public/:objectClass/Name/Id/SomeStyle/FileName")]
        public void WithSpecificLocationTemplate_ShouldReturnCorrectString(
            string locationTemplate,
            string className,
            string propertyName,
            string objectId,
            string style,
            string filename,
            string expected)
        {
            var options = new RegularLocationTemplateParserOptions();

            var formatter = new RegularLocationTemplateParser(Options.Create(options));

            var result = formatter.Parse(
                    className: className,
                    propertyName: propertyName,
                    objectId: objectId,
                    style: style,
                    filename: filename,
                    locationTemplate: locationTemplate);

            Assert.Equal(expected, result);
        }
    }
}
