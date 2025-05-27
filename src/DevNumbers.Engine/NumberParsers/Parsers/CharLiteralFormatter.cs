using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Formatter for character literals
/// </summary>
internal class CharLiteralFormatter : NumberFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.CharLiteral;

    public override bool SupportsBase(NumberBase format)
    {
        return format == NumberBase.Char; // Only supports char format
    }

    public override string GetDefaultAffix(NumberBase format)
    {
        return format == NumberBase.Char ? "'" : "";
    }

    public override string Format(BigInteger value, NumberBase format, FormatInfo formatInfo)
    {
        // Must be a character value
        if (format != NumberBase.Char || value < 0 || value > char.MaxValue)
            throw new ArgumentException("Value is not a valid character.");

        // Format as character with appropriate escaping
        return "'" + FormatChar((int)value) + "'";
    }
}