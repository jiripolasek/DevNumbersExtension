// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;



/// <summary>
/// Formatter for C-style prefix (0x, 0b, 0)
/// </summary>
internal abstract class CStylePrefixFormatterBase : NumberFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.CStylePrefix;

    protected CStylePrefixFormatterBase(char groupSeparator)
    {
        this.GroupSeparator = groupSeparator;
    }

    protected char GroupSeparator { get; set; }

    public override bool SupportsBase(NumberBase format)
    {
        return format switch
        {
            NumberBase.Decimal => true,
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
            NumberBase.Decimal => "",
            NumberBase.Hexadecimal => "0x",
            NumberBase.Binary => "0b",
            NumberBase.Octal => "0",
            _ => ""
        };
    }

    public override string Format(BigInteger value, NumberBase format, FormatInfo formatInfo)
    {
        var style = BigIntegerBaseStyle.FromFormatInfo(format, formatInfo) with { GroupSeparator = this.GroupSeparator, AllowGroupSeparator = this.GroupSeparator != (char)0 };
        string baseValue = ConvertToBase(value, style);
        string prefix = this.GetDefaultAffix(format);

        if (this.SupportedStyle == FormatStyle.CSharpStyle && baseValue.Length > style.GroupSize && !string.IsNullOrWhiteSpace(prefix))
        {
            prefix += this.GroupSeparator;
        }

        return prefix + baseValue;
    }
}

internal class CSharpStylePrefixFormatter : CStylePrefixFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.CSharpStyle;

    public CSharpStylePrefixFormatter() : base('_') { }
}

internal class CppStylePrefixFormatter : CStylePrefixFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.CppStyle;
    public CppStylePrefixFormatter() : base('\'') { }
}

internal class CStylePrefixFormatter : CStylePrefixFormatterBase
{
    public override FormatStyle SupportedStyle => FormatStyle.CStylePrefix;

    public CStylePrefixFormatter() : base((char)0) { }
}