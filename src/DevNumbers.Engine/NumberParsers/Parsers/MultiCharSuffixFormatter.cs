using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Formatter for multi-character suffix (bin, oct, hex, dec)
/// </summary>
internal class MultiCharSuffixFormatter : NumberFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.MultiCharSuffix;

    public override bool SupportsBase(NumberBase format)
    {
        return format != NumberBase.Char; // All numeric formats support multi-char suffix
    }

    public override string GetDefaultAffix(NumberBase format)
    {
        return format switch
        {
            NumberBase.Hexadecimal => "hex",
            NumberBase.Binary => "bin",
            NumberBase.Octal => "oct",
            NumberBase.Decimal => "dec",
            _ => ""
        };
    }

    public override string Format(BigInteger value, NumberBase format, FormatInfo formatInfo)
    {
        var style = BigIntegerBaseStyle.FromFormatInfo(format, formatInfo) with { AllowGroupSeparator = false };
        string baseValue = ConvertToBase(value, style);
        string suffix = this.GetDefaultAffix(format);
        return baseValue + suffix;
    }
}