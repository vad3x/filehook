using Microsoft.Extensions.Options;
using Xunit;

namespace Filehook.Core.Tests
{
    public class KebabLocationParamFormatter_Format
    {
        [Theory]
        [InlineData("paramName", null)]
        [InlineData("ParamName", null)]
        [InlineData("param-name", null)]
        [InlineData("param_name", null)]

        [InlineData("paramNameSomePosfix", "SomePosfix")]
        [InlineData("ParamNameSomePosfix", "SomePosfix")]
        [InlineData("param-name-some-posfix", "-some-posfix")]
        [InlineData("param_name_some_posfix", "_some_posfix")]
        public void ShouldReturnCorrectString(string param, string postfix)
        {
            var options = new KebabLocationParamFormatterOptions
            {
                Postfix = postfix
            };

            var formatter = new KebabLocationParamFormatter(Options.Create(options));

            var result = formatter.Format(param);

            Assert.Equal("param-name", result);
        }
    }
}
