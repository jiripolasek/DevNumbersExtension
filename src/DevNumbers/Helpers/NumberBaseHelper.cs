// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Resources;

namespace JPSoftworks.DevNumbers.Helpers;

internal static class NumberBaseHelper
{
    internal static string BaseToString(NumberBase numberBase)
    {
        return numberBase switch
        {
            NumberBase.Decimal => Strings.NumberBase_DecimalLabel!,
            NumberBase.Hexadecimal => Strings.NumberBase_Hexadecimal!,
            NumberBase.Binary => Strings.NumberBase_Binary!,
            NumberBase.Octal => Strings.NumberBase_Octal!,
            NumberBase.Char => Strings.NumberBase_Character!,
            _ => numberBase.ToString().ToLowerInvariant()
        };
    }
}