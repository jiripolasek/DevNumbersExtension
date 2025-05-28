// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers;


/// <summary>
/// Main parser class that coordinates the individual format parsers and formatters
/// </summary>
public static class NumberParser
{
    // Registry of parsers and formatters
    private static readonly IReadOnlyList<INumberParser> Parsers = (List<INumberParser>)
    [
        new CharLiteralParser(),

        new CSharpStylePrefixParser(),
        new CppStylePrefixParser(),
        new SingleCharPrefixParser(),
        new SpecialCharPrefixParser(),
        new VBStylePrefixParser(),
        new AdaStylePrefixParser(),
        new RStylePrefixParser(),
        new MultiCharSuffixParser(),
        new SingleCharSuffixParser(),

        new StandardFormatParser(),
    ];

    private static readonly INumberFormatter[] FormattersSource =
    [
        new StandardFormatter(),
        new CSharpStylePrefixFormatter(),
        new CppStylePrefixFormatter(),
        new CStylePrefixFormatter(),
        new SingleCharSuffixFormatter(),
        new MultiCharSuffixFormatter(),
        new SingleCharPrefixFormatter(),
        new SpecialCharPrefixFormatter(),
        new VBStylePrefixFormatter(),
        new AdaStylePrefixFormatter(),
        new RStylePrefixFormatter(),
        new CharLiteralFormatter()
    ];


    private static readonly Dictionary<FormatStyle, INumberFormatter> Formatters;

    static NumberParser()
    {
        Formatters = FormattersSource.ToDictionary(static t => t.SupportedStyle, static t => t);
    }



    /// <summary>
    /// Parses a string into a numeric value with format information
    /// </summary>
    public static InputFormatParserResult? Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be null or empty", nameof(input));

        var trimmedInput = NormalizeInput(input);

        foreach (var parser in Parsers)
        {
            if (parser.TryParse(trimmedInput, out var result))
            {
                return result;
            }
        }

        throw new FormatException($"Unable to parse the input as any known number format: {input}");
    }


    private static string NormalizeInput(string input)
    {
        var trimmedInput = input.Trim();
        return trimmedInput.Length >= 2 && trimmedInput[0] == '\'' && trimmedInput[^1] == '\''
            ? trimmedInput
            : string.Concat(trimmedInput.Where(static c => !char.IsWhiteSpace(c)));
    }

    public static bool TryParse(string input, out InputFormatParserResult? result)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            result = null;
            return false;
        }

        var trimmedInput = NormalizeInput(input);
        
        foreach (var parser in Parsers)
        {
            if (parser.TryParse(trimmedInput, out result))
            {
                return true;
            }
        }
        result = null;
        return false;
    }

    /// <summary>
    /// Gets the preferred output format style for a given input format
    /// </summary>
    public static FormatInfo GetPreferredFormatStyle(NumberBase targetFormat, FormatInfo? currentFormat = null)
    {
        // If we have a current format, try to match it
        if (currentFormat != null)
        {
            // For same format, keep the same style
            if (targetFormat == currentFormat.Format)
                return currentFormat;

            // Find a formatter that supports the target format and current style
            if (Formatters.TryGetValue(currentFormat.Style, out var formatter) &&
                formatter.SupportsBase(targetFormat))
            {
                return new FormatInfo(
                    currentFormat.Style,
                    targetFormat == NumberBase.Hexadecimal ? currentFormat.IsUpperCase : false
                );
            }

            // Fall back to default style
            return GetDefaultStyle(targetFormat, targetFormat == NumberBase.Hexadecimal ? currentFormat.IsUpperCase : false);
        }

        // No current format - use defaults
        return GetDefaultStyle(targetFormat, false);
    }

    /// <summary>
    /// Gets the default format style for a given format
    /// </summary>
    private static FormatInfo GetDefaultStyle(NumberBase format, bool isUpperCase)
    {
        return format switch
        {
            NumberBase.Decimal => new FormatInfo(FormatStyle.Standard),
            NumberBase.Hexadecimal => new FormatInfo(FormatStyle.CStylePrefix, isUpperCase),
            NumberBase.Binary => new FormatInfo(FormatStyle.CStylePrefix),
            NumberBase.Octal => new FormatInfo(FormatStyle.CStylePrefix),
            NumberBase.Char => new FormatInfo(FormatStyle.CharLiteral),
            _ => new FormatInfo(FormatStyle.Standard)
        };
    }

    /// <summary>
    /// Formats a value according to the specified format style
    /// </summary>
    public static string Format(BigInteger value, NumberBase @base, FormatInfo formatInfo)
    {
        if (!Formatters.TryGetValue(formatInfo.Style, out var formatter))
            throw new ArgumentException($"Unsupported format style: {formatInfo.Style}");

        if (!formatter.SupportsBase(@base))
            throw new ArgumentException($"Format style {formatInfo.Style} does not support {@base} format");

        return formatter.Format(value, @base, formatInfo);
    }

    /// <summary>
    /// Formats a value according to the specified format style
    /// </summary>
    public static string FormatWithFallback(BigInteger value, NumberBase @base, FormatInfo formatInfo)
    {
        if (!Formatters.TryGetValue(formatInfo.Style, out var formatter))
            throw new ArgumentException($"Unsupported format style: {formatInfo.Style}");

        if (!formatter.SupportsBase(@base))
        {
            formatter = Formatters.FirstOrDefault(pair => pair.Value.SupportsBase(@base)).Value;
        }

        formatter ??= Formatters[0];

        return formatter.Format(value, @base, formatInfo);
    }

    /// <summary>
    /// Converts between different numeric formats
    /// </summary>
    public static InputFormatParserResult Convert(InputFormatParserResult result, NumberBase targetFormat)
    {
        FormatInfo newFormatInfo = GetPreferredFormatStyle(targetFormat, result.FormatInfo);
        return new InputFormatParserResult(
            FormatWithFallback(result.Value, targetFormat, newFormatInfo),
            targetFormat,
            result.Value,
            false, // Not implicit when explicitly converted
            newFormatInfo
        );
    }

    /// <summary>
    /// Converts to a specified format with a specific format style
    /// </summary>
    public static InputFormatParserResult Convert(InputFormatParserResult result, NumberBase targetFormat, FormatStyle targetStyle)
    {
        // Get the formatter for the target style
        if (!Formatters.TryGetValue(targetStyle, out var formatter))
            throw new ArgumentException($"Unsupported format style: {targetStyle}");

        // Check if formatter supports the target format
        if (!formatter.SupportsBase(targetFormat))
            throw new ArgumentException($"Format style {targetStyle} does not support {targetFormat} format");

        // Get default affix for this style and format
        string affix = formatter.GetDefaultAffix(targetFormat);

        // Create format info
        FormatInfo newFormatInfo = new FormatInfo(
            targetStyle,
            targetFormat == NumberBase.Hexadecimal ? result.FormatInfo.IsUpperCase : false
        );

        // Format the value
        string formattedValue = formatter.Format(result.Value, targetFormat, newFormatInfo);

        // Create new result
        return new InputFormatParserResult(
            formattedValue,
            targetFormat,
            result.Value,
            false, // Not implicit when explicitly converted
            newFormatInfo
        );
    }

    /// <summary>
    /// Gets all supported format styles for a specific format
    /// </summary>
    public static IEnumerable<FormatStyle> GetSupportedStyles(NumberBase format)
    {
        return Formatters
            .Where(kv => kv.Value.SupportsBase(format))
            .Select(static kv => kv.Key);
    }

    /// <summary>
    /// Creates a formatter for a specific style
    /// </summary>
    public static INumberFormatter GetFormatter(FormatStyle style)
    {
        if (Formatters.TryGetValue(style, out var formatter))
            return formatter;

        throw new ArgumentException($"Unsupported format style: {style}");
    }
}