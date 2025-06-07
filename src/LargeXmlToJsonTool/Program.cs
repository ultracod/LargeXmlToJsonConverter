using System.CommandLine;
using System.CommandLine.Binding;
using LargeXmlToJsonConverter;
using Microsoft.Extensions.Logging;

var inputOption = new Option<string>("--input", description: "Path to input XML file") { IsRequired = true };
var outputOption = new Option<string>("--output", description: "Path for output JSON file") { IsRequired = true };
var indentOption = new Option<bool>("--indent", () => false, "Indent JSON output");
var rootCommand = new RootCommand("Large XML to JSON Converter")
{
    inputOption,
    outputOption,
    indentOption
};

rootCommand.SetHandler(async (string input, string output, bool indent) =>
{
    using var inputStream = File.OpenRead(input);
    using var outputStream = File.Create(output);
    var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole());
    var logger = loggerFactory.CreateLogger<XmlToJsonConverter>();
    var converter = new XmlToJsonConverter(logger);
    var options = new ConversionOptions
    {
        JsonWriterOptions = new System.Text.Json.JsonWriterOptions { Indented = indent }
    };
    await converter.ConvertAsync(inputStream, outputStream, options);
}, inputOption, outputOption, indentOption);

return await rootCommand.InvokeAsync(args);
