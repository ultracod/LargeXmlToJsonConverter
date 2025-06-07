using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LargeXmlToJsonConverter.Tests
{
    public class CustomConverterTests
    {
        [Fact]
        public async Task UsesCustomConverter()
        {
            var xml = "<root><child attr='1'>text</child></root>";
            await using var input = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            await using var output = new MemoryStream();
            var converter = new LargeXmlToJsonConverter.XmlToJsonConverter();
            var options = new LargeXmlToJsonConverter.ConversionOptions
            {
                NodeConverter = (reader, writer, ct) =>
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name == "child")
                    {
                        writer.WriteString("custom", reader.GetAttribute("attr") ?? string.Empty);
                        reader.Skip();
                        return Task.FromResult(true);
                    }
                    return Task.FromResult(false);
                }
            };
            await converter.ConvertAsync(input, output, options, CancellationToken.None);
            var json = Encoding.UTF8.GetString(output.ToArray());
            Assert.Contains("custom", json);
            Assert.DoesNotContain("\"child\"", json);
        }
    }
}
