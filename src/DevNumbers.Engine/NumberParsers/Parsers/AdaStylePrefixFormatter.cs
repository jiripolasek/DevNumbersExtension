using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Formatter for Ada-style base notation (16#ABCD#, 2#1010#, 8#777#, 10#1234#)
/// </summary>
internal class AdaStylePrefixFormatter : NumberFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.AdaStylePrefix;

    public override bool SupportsBase(NumberBase format)
    {
        return format != NumberBase.Char; // All numeric formats support Ada style
    }

    public override string GetDefaultAffix(NumberBase format)
    {
        return format switch
        {
            NumberBase.Hexadecimal => "16#",
            NumberBase.Binary => "2#",
            NumberBase.Octal => "8#",
            NumberBase.Decimal => "10#",
            _ => ""
        };
    }

    public override string Format(BigInteger value, NumberBase format, FormatInfo formatInfo)
    {
        var style = BigIntegerBaseStyle.FromFormatInfo(format, formatInfo) with { AllowGroupSeparator = false };
        string baseValue = ConvertToBase(value, style);
        string prefix = this.GetDefaultAffix(format);
        return prefix + baseValue + "#";
    }
}