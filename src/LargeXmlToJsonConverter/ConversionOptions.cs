using System.Text.Json;

namespace LargeXmlToJsonConverter
{
    /// <summary>
    /// Options controlling XML to JSON conversion.
    /// </summary>
    public class ConversionOptions
    {
        /// <summary>
        /// Gets a default set of options.
        /// </summary>
        public static ConversionOptions Default { get; } = new ConversionOptions();

        /// <summary>
        /// Options for the underlying JSON writer.
        /// </summary>
        public JsonWriterOptions JsonWriterOptions { get; set; } = new JsonWriterOptions { Indented = false };

        /// <summary>
        /// Optional custom node converter invoked for each XML node. Should return
        /// <c>true</c> if the node was handled and no further processing is required.
        /// </summary>
        public Func<XmlReader, Utf8JsonWriter, CancellationToken, Task<bool>>? NodeConverter { get; set; }
    }
}
