// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Formatter for Visual Basic style prefixes (&amp;H, &amp;B, &amp;O)
/// </summary>
internal class VBStylePrefixFormatter : NumberFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.VBStylePrefix;

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
            NumberBase.Hexadecimal => "&H",
            NumberBase.Binary => "&B",
            NumberBase.Octal => "&O",
            _ => ""
        };
    }

    public override string Format(BigInteger value, NumberBase format, FormatInfo formatInfo)
    {
        var style = BigIntegerBaseStyle.FromFormatInfo(format, formatInfo) with { AllowGroupSeparator = true, GroupSeparator = '_' };
        string baseValue = ConvertToBase(value, style);
        string prefix = this.GetDefaultAffix(format);
        return prefix + baseValue;
    }
}