using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Formatter for R-style base notation (16rABCD, 2r1010, 8r777, 10r1234)
/// </summary>
internal class RStylePrefixFormatter : NumberFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.RStylePrefix;

    public override bool SupportsBase(NumberBase format)
    {
        return format != NumberBase.Char; // All numeric formats support R style
    }

    public override string GetDefaultAffix(NumberBase format)
    {
        return format switch
        {
            NumberBase.Hexadecimal => "16r",
            NumberBase.Binary => "2r",
            NumberBase.Octal => "8r",
            NumberBase.Decimal => "10r",
            _ => ""
        };
    }

    public override string Format(BigInteger value, NumberBase format, FormatInfo formatInfo)
    {
        var style = BigIntegerBaseStyle.FromFormatInfo(format, formatInfo) with { AllowGroupSeparator = false };
        string baseValue = ConvertToBase(value, style);
        string prefix = this.GetDefaultAffix(format);
        return prefix + baseValue;
    }
}