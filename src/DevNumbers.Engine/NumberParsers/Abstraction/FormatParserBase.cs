using System.Globalization;
using System.Numerics;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

/// <summary>
/// Interface for parsing numbers in a specific style
/// </summary>
public interface INumberParser
{
    /// <summary>
    /// Try to parse a string as a number in this parser's style
    /// </summary>
    bool TryParse(string input, out InputFormatParserResult? result);

    /// <summary>
    /// Get the format style supported by this parser
    /// </summary>
    FormatStyle SupportedStyle { get; }
}

/// <summary>
/// Interface for formatting numbers in a specific style
/// </summary>
public interface INumberFormatter
{
    /// <summary>
    /// Format a number according to this formatter's style
    /// </summary>
    string Format(BigInteger value, NumberBase format, FormatInfo formatInfo);

    /// <summary>
    /// Get the format style supported by this formatter
    /// </summary>
    FormatStyle SupportedStyle { get; }

    /// <summary>
    /// Check if this formatter supports the given format
    /// </summary>
    bool SupportsBase(NumberBase format);

    /// <summary>
    /// Get the default affix (prefix/suffix) for a format
    /// </summary>
    string GetDefaultAffix(NumberBase format);
}

/// <summary>
/// Base class for number parsers
/// </summary>
public abstract class NumberParserBase : INumberParser
{
    /// <summary>
    /// Try to parse a string as a number in this parser's style
    /// </summary>
    public abstract bool TryParse(string input, out InputFormatParserResult? result);

    /// <summary>
    /// The format style supported by this parser
    /// </summary>
    public abstract FormatStyle SupportedStyle { get; }

    /// <summary>
    /// Parse a hexadecimal string
    /// </summary>
    protected bool TryParseHexadecimal(string value, out BigInteger result)
    {
        return BigIntegerBaseConverter.TryParse(value, BigIntegerBaseStyle.Hexadecimal, out result);
    }

    /// <summary>
    /// Parse a binary string
    /// </summary>
    protected bool TryParseBinary(string value, out BigInteger result)
    {
        return BigIntegerBaseConverter.TryParse(value, BigIntegerBaseStyle.Binary, out result);
    }

    /// <summary>
    /// Parse an octal string
    /// </summary>
    protected bool TryParseOctal(string value, out BigInteger result)
    {
        return BigIntegerBaseConverter.TryParse(value, BigIntegerBaseStyle.Octal, out result);
    }

    /// <summary>
    /// Parse a decimal string
    /// </summary>
    protected bool TryParseDecimal(string value, out BigInteger result)
    {
        return BigIntegerBaseConverter.TryParse(value, BigIntegerBaseStyle.Decimal, out result);
    }
}

/// <summary>
/// Base class for number formatters
/// </summary>
public abstract class NumberFormatterBase : INumberFormatter
{
    /// <summary>
    /// Format a number according to this formatter's style
    /// </summary>
    public abstract string Format(BigInteger value, NumberBase format, FormatInfo formatInfo);

    /// <summary>
    /// The format style supported by this formatter
    /// </summary>
    public abstract FormatStyle SupportedStyle { get; }

    /// <summary>
    /// Check if this formatter supports the given format
    /// </summary>
    public abstract bool SupportsBase(NumberBase format);

    /// <summary>
    /// Get the default affix (prefix/suffix) for a format
    /// </summary>
    public abstract string GetDefaultAffix(NumberBase format);

    /// <summary>
    /// Basic number conversion to string
    /// </summary>
    protected static string ConvertToBase(BigInteger value, BigIntegerBaseStyle style)
    {
        return BigIntegerBaseConverter.ToString(value, style);
    }

    /// <summary>
    /// Format a character value
    /// </summary>
    protected static string FormatChar(int value)
    {
        return value switch
        {
            '\'' => @"\'",  // Single quote
            '\"' => "\\\"", // Double quote
            '\\' => @"\\",  // Backslash
            '\0' => @"\0",  // Null
            '\a' => @"\a",  // Alert (bell)
            '\b' => @"\b",  // Backspace
            '\f' => @"\f",  // Form feed
            '\n' => @"\n",  // New line
            '\r' => @"\r",  // Carriage return
            '\t' => @"\t",  // Horizontal tab
            '\v' => @"\v",  // Vertical tab
            < 32 or > 126 => $"\\u{value:X4}", // Non-printable as Unicode escape
            _ => ((char)value).ToString() // Normal character
        };
    }
}