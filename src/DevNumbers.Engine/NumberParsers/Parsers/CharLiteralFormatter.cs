using System.Numerics;
using System.Text;
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
        if (format != NumberBase.Char || value < 0)
            throw new ArgumentException("Value is not a valid character.");

        if (value < char.MaxValue)
        {
            return $"'{FormatChar((int)value)}'";
        }
        else if(value < int.MaxValue)
        {
            var codePoint = (int)value;
            return $"'{char.ConvertFromUtf32(codePoint)}'";
        }
        else
        {
            throw new ArgumentException("Value is outside the valid range for a character.");
        }
    }
}