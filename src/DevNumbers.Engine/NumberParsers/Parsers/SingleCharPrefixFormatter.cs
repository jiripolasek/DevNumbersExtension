using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Formatter for single character prefix (h, x, b, o, d)
/// </summary>
internal class SingleCharPrefixFormatter : NumberFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.SingleCharPrefix;

    public override bool SupportsBase(NumberBase format)
    {
        return format is NumberBase.Hexadecimal or NumberBase.Octal;
    }

    public override string GetDefaultAffix(NumberBase format)
    {
        return format switch
        {
            NumberBase.Hexadecimal => "h",
            NumberBase.Octal => "o",
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