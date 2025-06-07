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
    }
}
