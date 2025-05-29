// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Resources;

namespace JPSoftworks.DevNumbers.Helpers;

internal static class FormatStyleHelper
{
    internal static string FormatStyleToString(FormatStyle formatStyle)
    {
        return formatStyle switch
        {
            FormatStyle.Standard => Strings.StyleName_Standard_Inner!,
            FormatStyle.CStylePrefix => Strings.StyleName_CStyle_Inner!,
            FormatStyle.CppStyle => Strings.StyleName_CppStyle_Inner!,
            FormatStyle.CSharpStyle => Strings.StyleName_CSharp_Inner!,
            FormatStyle.SingleCharSuffix => Strings.StyleName_SingleCharSuffix_Inner!,
            FormatStyle.MultiCharSuffix => Strings.StyleName_MultiCharSuffix_Inner!,
            FormatStyle.SingleCharPrefix => Strings.StyleName_SingleCharPrefix_Inner!,
            FormatStyle.SpecialCharPrefix => Strings.StyleName_SpecialCharPrefix_Inner!,
            FormatStyle.VBStylePrefix => Strings.StyleName_VBStyle_Inner!,
            FormatStyle.AdaStylePrefix => Strings.StyleName_AdaStyle_Inner!,
            FormatStyle.RStylePrefix => Strings.StyleName_RStyle_Inner!,
            _ => formatStyle.ToString().ToLowerInvariant()
        };
    }
}