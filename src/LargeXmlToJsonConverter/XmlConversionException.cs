using System;

namespace LargeXmlToJsonConverter
{
    /// <summary>
    /// Represents errors that occur during XML to JSON conversion.
    /// </summary>
    public class XmlConversionException : Exception
    {
        public XmlConversionException(string message, Exception? inner = null)
            : base(message, inner)
        {
        }
    }
}
