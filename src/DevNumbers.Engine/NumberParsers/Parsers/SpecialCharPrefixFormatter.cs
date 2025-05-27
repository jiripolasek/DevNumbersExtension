using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Formatter for special character prefix (#, $, %, @)
/// </summary>
internal class SpecialCharPrefixFormatter : NumberFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.SpecialCharPrefix;

    public override bool SupportsBase(NumberBase format)
    {
        return format switch
        {
            NumberBase.Hexadecimal => true,
            NumberBase.Binary => true,
            NumberBase.Octal => true,
            _ => false
        };
    }

    public override string GetDefaultAffix(NumberBase format)
    {
        return format switch
        {
            NumberBase.Hexadecimal => "#",
            NumberBase.Binary => "%",
            NumberBase.Octal => "@",
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