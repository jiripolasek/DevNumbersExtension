using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Formatter for standard format with no prefix or suffix
/// </summary>
internal class StandardFormatter : NumberFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.Standard;

    public override bool SupportsBase(NumberBase format)
    {
        return format != NumberBase.Char; // Standard doesn't support char format
    }

    public override string GetDefaultAffix(NumberBase format)
    {
        return ""; // No affixes in standard format
    }

    public override string Format(BigInteger value, NumberBase format, FormatInfo formatInfo)
    {
        var style = BigIntegerBaseStyle.FromFormatInfo(format, formatInfo);
        return ConvertToBase(value, style);
    }
}