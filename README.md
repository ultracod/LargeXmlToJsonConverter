# LargeXmlToJsonConverter

A streaming XML to JSON conversion library with a CLI tool. Designed to handle very large XML files (up to multiple gigabytes) using `XmlReader` and `Utf8JsonWriter` without loading entire documents into memory.

## Projects

- **LargeXmlToJsonConverter** – core class library.
- **LargeXmlToJsonTool** – command line interface.
- **LargeXmlToJsonConverter.Tests** – xUnit tests.

## Building

Use the .NET 6 SDK:

```bash
# restore and build
 dotnet build
```

## CLI Usage

```bash
largexmltojsontool --input input.xml --output output.json --indent
```

## Testing

Run unit tests with:

```bash
 dotnet test
```

