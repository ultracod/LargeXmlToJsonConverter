using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace LargeXmlToJsonConverter
{
    /// <summary>
    /// Streaming XML to JSON converter.
    /// </summary>
    public class XmlToJsonConverter
    {
        private readonly ILogger<XmlToJsonConverter> _logger;

        public XmlToJsonConverter(ILogger<XmlToJsonConverter>? logger = null)
        {
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<XmlToJsonConverter>.Instance;
        }

        /// <summary>
        /// Convert XML from input stream to JSON on output stream using provided options.
        /// </summary>
        public void Convert(Stream input, Stream output, ConversionOptions? options = null)
        {
            options ??= ConversionOptions.Default;
            using var reader = XmlReader.Create(input, new XmlReaderSettings { Async = false });
            using var writer = new Utf8JsonWriter(output, options.JsonWriterOptions);
            ConvertInternal(reader, writer, options, CancellationToken.None);
            writer.Flush();
        }

        /// <summary>
        /// Asynchronously convert XML to JSON.
        /// </summary>
        public async Task ConvertAsync(Stream input, Stream output, ConversionOptions? options = null, CancellationToken cancellationToken = default)
        {
            options ??= ConversionOptions.Default;
            using var reader = XmlReader.Create(input, new XmlReaderSettings { Async = true });
            using var writer = new Utf8JsonWriter(output, options.JsonWriterOptions);
            await ConvertInternalAsync(reader, writer, options, cancellationToken).ConfigureAwait(false);
            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        private void ConvertInternal(XmlReader reader, Utf8JsonWriter writer, ConversionOptions options, CancellationToken ct)
        {
            try
            {
                WriteJson(reader, writer, options, ct).GetAwaiter().GetResult();
            }
            catch (OutOfMemoryException oom)
            {
                throw new XmlConversionException("Ran out of memory during conversion", oom);
            }
            catch (XmlException xe)
            {
                throw new XmlConversionException($"Malformed XML at line {xe.LineNumber}, position {xe.LinePosition}", xe);
            }
        }

        private async Task ConvertInternalAsync(XmlReader reader, Utf8JsonWriter writer, ConversionOptions options, CancellationToken ct)
        {
            try
            {
                await WriteJson(reader, writer, options, ct).ConfigureAwait(false);
            }
            catch (OutOfMemoryException oom)
            {
                throw new XmlConversionException("Ran out of memory during conversion", oom);
            }
            catch (XmlException xe)
            {
                throw new XmlConversionException($"Malformed XML at line {xe.LineNumber}, position {xe.LinePosition}", xe);
            }
        }

        private async Task WriteJson(XmlReader reader, Utf8JsonWriter writer, ConversionOptions options, CancellationToken ct)
        {
            var depth = 0;
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                ct.ThrowIfCancellationRequested();
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        writer.WriteStartObject();
                        writer.WriteString("name", reader.Name);
                        if (reader.HasAttributes)
                        {
                            writer.WriteStartObject("attributes");
                            while (reader.MoveToNextAttribute())
                            {
                                writer.WriteString(reader.Name, reader.Value);
                            }
                            writer.WriteEndObject();
                            reader.MoveToElement();
                        }
                        if (reader.IsEmptyElement)
                        {
                            writer.WriteEndObject();
                        }
                        depth++;
                        break;
                    case XmlNodeType.Text:
                        writer.WriteString("text", reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        writer.WriteEndObject();
                        depth--;
                        break;
                }
            }
            while (depth > 0)
            {
                writer.WriteEndObject();
                depth--;
            }
        }
    }
}

