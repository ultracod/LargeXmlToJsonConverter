using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace LargeXmlToJsonConverter.Tests
{
    public class SimpleConversionTests
    {
        [Fact]
        public async Task ConvertsSimpleXml()
        {
            var xml = "<root><child attr='1'>text</child></root>";
            await using var input = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            await using var output = new MemoryStream();
            var converter = new LargeXmlToJsonConverter.XmlToJsonConverter();
            await converter.ConvertAsync(input, output);
            var json = Encoding.UTF8.GetString(output.ToArray());
            Assert.Contains("\"child\"", json);
        }
    }
}
