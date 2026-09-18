// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Helpers;

namespace JPSoftworks.DevNumbers.Test;

public class NumberQueryTest
{
    [Theory]
    [InlineData("0xFF /length:8", 255, 255, 8)]
    [InlineData("0x1FF /length:8", 511, 255, 8)]
    [InlineData("/length:byte 0x1FF", 511, 255, 8)]
    [InlineData("-1 /length:8", -1, 255, 8)]
    [InlineData("0x100 /length:8", 256, 0, 8)]
    [InlineData("0x1'0000 /length:word", 65536, 0, 16)]
    [InlineData("'/' /length:8", 47, 47, 8)]
    [InlineData("-128", -128, -128, 8)]
    [InlineData("-129", -129, -129, 16)]
    [InlineData("255", 255, 255, -1)]
    public void Parse_Query_PreservesOriginalAndAppliesBitLength(
        string input, int originalValue, int expectedValue, int expectedBitLength)
    {
        var parsedQuery = SwitchParser.Parse(input);
        var result = NumberQuery.Parse(parsedQuery, null);

        Assert.Empty(parsedQuery.Errors);
        Assert.Equal(new BigInteger(originalValue), result.Number.Value);
        Assert.Equal(parsedQuery.Query, result.Number.RawValue);
        Assert.Equal(new BigInteger(expectedValue), result.Value);
        Assert.Equal(expectedBitLength, result.BitLength);
    }

    [Theory]
    [InlineData("0xff", null, FormatStyle.CSharpStyle)]
    [InlineData("0x1'0000", null, FormatStyle.CppStyle)]
    [InlineData("255", null, FormatStyle.Standard)]
    [InlineData("0xff /length:8", FormatStyle.VBStylePrefix, FormatStyle.VBStylePrefix)]
    [InlineData("65536", FormatStyle.CppStyle, FormatStyle.CppStyle)]
    public void Parse_Query_SelectsConfiguredOrInferredStyle(string input, FormatStyle? configuredStyle, FormatStyle expectedStyle)
    {
        var result = NumberQuery.Parse(SwitchParser.Parse(input), configuredStyle);

        Assert.Equal(expectedStyle, result.FormatStyle);
    }

    [Theory]
    [InlineData("0xFF /length:8", null, NumberBase.Hexadecimal, "0xFF")]
    [InlineData("0x1FF /length:8", FormatStyle.CppStyle, NumberBase.Hexadecimal, "0xFF")]
    [InlineData("65536", FormatStyle.CppStyle, NumberBase.Hexadecimal, "0x1'0000")]
    [InlineData("0xFF /length:8", FormatStyle.VBStylePrefix, NumberBase.Hexadecimal, "&HFF")]
    [InlineData("0xFF /length:8", FormatStyle.VBStylePrefix, NumberBase.Decimal, "255")]
    [InlineData("255", FormatStyle.SingleCharPrefix, NumberBase.Binary, "1111 1111")]
    [InlineData("-1 /length:8", FormatStyle.CppStyle, NumberBase.Decimal, "255")]
    [InlineData("'/' /length:8", null, NumberBase.Hexadecimal, "2F")]
    [InlineData("12345", FormatStyle.CppStyle, NumberBase.Decimal, "12'345")]
    public void Parse_Query_UsesProcessedValueAndStyleForFormatting(
        string input, FormatStyle? configuredStyle, NumberBase numberBase, string expected)
    {
        var result = NumberQuery.Parse(SwitchParser.Parse(input), configuredStyle);

        var formatted = NumberParser.FormatWithFallback(result.Value, numberBase, new FormatInfo(result.FormatStyle));

        Assert.Equal(expected, formatted);
    }

    [Theory]
    [InlineData("0xFF /length:0")]
    [InlineData("0xFF /length:invalid")]
    [InlineData("0xFF /lenght:8")]
    public void Parse_InvalidSwitch_PreservesWarningAndUnmodifiedValue(string input)
    {
        var parsedQuery = SwitchParser.Parse(input);
        var result = NumberQuery.Parse(parsedQuery, FormatStyle.CppStyle);

        Assert.Single(parsedQuery.Errors);
        Assert.True(parsedQuery.HasErrors);
        Assert.Equal(new BigInteger(255), result.Value);
        Assert.Equal(-1, result.BitLength);
        Assert.Equal(FormatStyle.CppStyle, result.FormatStyle);
    }

    [Theory]
    [InlineData("0b102 /length:8")]
    [InlineData("hex /length:8")]
    public void Parse_InvalidNumber_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(() => NumberQuery.Parse(SwitchParser.Parse(input), null));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("/length:8")]
    public void Parse_MissingNumber_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => NumberQuery.Parse(SwitchParser.Parse(input), null));
    }
}