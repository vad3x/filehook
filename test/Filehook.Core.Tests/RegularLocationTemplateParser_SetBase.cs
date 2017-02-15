using Microsoft.Extensions.Options;
using Xunit;

namespace Filehook.Core.Tests
{
    public class RegularLocationTemplateParser_SetBase
    {
        [Theory]
        [InlineData(":base/public", "BasePath", "BasePath/public")]
        public void WithSpecificLocationTemplate_ShouldReturnCorrectString(
            string locationTemplate,
            string baseLocation,
            string expected)
        {
            var options = new RegularLocationTemplateParserOptions();

            var formatter = new RegularLocationTemplateParser(Options.Create(options));

            var result = formatter.SetBase(locationTemplate, baseLocation);

            Assert.Equal(expected, result);
        }
    }
}
