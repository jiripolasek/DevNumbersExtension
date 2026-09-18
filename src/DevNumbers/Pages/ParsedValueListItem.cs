// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;
using System.Numerics;
using JPSoftworks.DevNumbers.Engine;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Helpers;
using JPSoftworks.DevNumbers.Resources;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Pages;

internal sealed partial class ParsedValueListItem : ListItem
{
    public ParsedValueListItem(
        InputFormatParserResult parsedInput,
        int explicitBitLength)
    {
        var numberBase = parsedInput.NumberBase;
        this.Icon = Icons.Bullseye;

        var formattedValue = FormatValue(parsedInput.Value, parsedInput.NumberBase);

        this.Title = formattedValue;

        var parsedNumberMinimalBitLength = parsedInput.Value.GetBitLengthWithSign();
        var pow2BitLength = parsedInput.Value.GetNearestContainerBitLength();
        var realBitLength = parsedNumberMinimalBitLength > -1 ? parsedNumberMinimalBitLength : parsedInput.Value.GetBitLengthWithSign();

        var isTrimmed = explicitBitLength > -1 && (parsedInput.Value.GetBitLengthWithSign() > explicitBitLength);
        if (isTrimmed)
        {
            this.Title = Strings.OutOfBounds!;
            this.Subtitle += $"{formattedValue} doesn't fit {NamedBitLengths.GetName(explicitBitLength)} and will be truncated";
            this.Subtitle += Environment.NewLine;
            this.Icon = Icons.Warning;
        }

        this.Subtitle += $"interpreted as {NumberBaseHelper.BaseToString(numberBase)}";
        if (pow2BitLength == realBitLength)
        {
            this.Subtitle += $" • fits in {NamedBitLengths.GetName(pow2BitLength)} ({pow2BitLength} bits) • exact size {realBitLength} bits";
        }
        else
        {
            this.Subtitle += $" • fits in {NamedBitLengths.GetName(pow2BitLength)} ({pow2BitLength} bits) • minimal size {realBitLength} bits";
        }

        this.Command = new NoOpCommand();
    }

    private static string FormatValue(BigInteger value, NumberBase format)
    {
        return format switch
        {
            NumberBase.Decimal => BigIntegerBaseConverter.ToString(value, BigIntegerBaseStyle.Decimal),
            NumberBase.Hexadecimal => BigIntegerBaseConverter.ToString(value, BigIntegerBaseStyle.HexadecimalUppercase),
            NumberBase.Binary => BigIntegerBaseConverter.ToString(value, BigIntegerBaseStyle.Binary),
            NumberBase.Octal => BigIntegerBaseConverter.ToString(value, BigIntegerBaseStyle.Octal),
            NumberBase.Char => BigIntegerCharacterInference.TryParseAsUtf32(value)?.DisplayCharacter ?? value.ToString(CultureInfo.InvariantCulture),
            _ => value.ToString(CultureInfo.InvariantCulture)
        };
    }
}