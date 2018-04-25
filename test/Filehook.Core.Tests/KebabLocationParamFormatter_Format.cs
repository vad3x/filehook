using Xunit;

namespace Filehook.Core.Tests
{
    public class KebabLocationParamFormatter_Format
    {
        [Theory]
        [InlineData("paramName")]
        [InlineData("ParamName")]
        [InlineData("param-name")]
        [InlineData("param_name")]
        public void ShouldReturnCorrectString(string param)
        {
            var formatter = new KebabLocationParamFormatter();

            var result = formatter.Format(param);

            Assert.Equal("param-name", result);
        }
    }
}
