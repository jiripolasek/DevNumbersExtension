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

public class SwitchParserTest
{
    [Theory]
    [InlineData("'/'", "'/'", -1, null, 47)]
    [InlineData("'/' /length:8", "'/'", 8, null, 47)]
    [InlineData("/length:8 '/'", "'/'", 8, null, 47)]
    [InlineData("'/'/length:8/signed", "'/'", 8, true, 47)]
    [InlineData("/unsigned '/' /length:byte", "'/'", 8, false, 47)]
    [InlineData("'\\'' /length:8", "'\\''", 8, null, 39)]
    [InlineData("'\\\\' /length:8", "'\\\\'", 8, null, 92)]
    [InlineData("' ' /length:8", "' '", 8, null, 32)]
    [InlineData("'\t' /length:8", "'\t'", 8, null, 9)]
    [InlineData("'\u00A0' /length:16", "'\u00A0'", 16, null, 160)]
    [InlineData("'\\n' /signed", "'\\n'", -1, true, 10)]
    public void Parse_CharacterLiteral_PreservesLiteralAndReadsSwitches(
        string input, string expectedQuery, int expectedBitLength, bool? expectedSigned, int expectedValue)
    {
        var result = SwitchParser.Parse(input);

        Assert.Equal(expectedQuery, result.Query);
        Assert.Empty(result.Errors);
        Assert.Equal(expectedBitLength, result.Options.BitLength);
        Assert.Equal(expectedSigned, result.Options.Signed);

        var number = NumberParser.Parse(result.Query);
        Assert.NotNull(number);
        Assert.Equal(NumberBase.Char, number.NumberBase);
        Assert.Equal(new BigInteger(expectedValue), number.Value);
    }

    [Theory]
    [InlineData("0x1'0000 /length:32", "0x1'0000", 32, 65536L)]
    [InlineData("0x1'0000'0000/length:64", "0x1'0000'0000", 64, 4294967296L)]
    [InlineData("1'000 /length:16", "1'000", 16, 1000L)]
    public void Parse_CppDigitSeparators_PreservesNumberAndReadsSwitches(
        string input, string expectedQuery, int expectedBitLength, long expectedValue)
    {
        var result = SwitchParser.Parse(input);

        Assert.Equal(expectedQuery, result.Query);
        Assert.Empty(result.Errors);
        Assert.Equal(expectedBitLength, result.Options.BitLength);
        Assert.Equal(new BigInteger(expectedValue), NumberParser.Parse(result.Query)?.Value);
    }

    [Theory]
    [InlineData("'  ' /length:8", "'  '")]
    [InlineData("'/signed' /length:8", "'/signed'")]
    public void Parse_InvalidQuotedLiteral_PreservesTextForNumberValidation(string input, string expectedQuery)
    {
        var result = SwitchParser.Parse(input);

        Assert.Equal(expectedQuery, result.Query);
        Assert.Empty(result.Errors);
        Assert.Equal(8, result.Options.BitLength);
        Assert.Null(result.Options.Signed);
        Assert.Throws<FormatException>(() => NumberParser.Parse(result.Query));
    }

    [Theory]
    [InlineData("'/' /lenght:8", "UnknownSwitch")]
    [InlineData("'/' /length", "MissingArgument")]
    [InlineData("'/' /length:0", "InvalidValue")]
    [InlineData("'/' /signed:yes", "UnexpectedArgument")]
    [InlineData("'/' /", "MissingSwitchName")]
    [InlineData("'/' /bad!name", "InvalidSwitchName")]
    public void Parse_InvalidSwitchAfterLiteral_PreservesErrorLocation(string input, string expectedError)
    {
        var result = SwitchParser.Parse(input);

        Assert.Equal("'/'", result.Query);
        var error = Assert.Single(result.Errors);
        Assert.Equal(expectedError, error.Type.ToString());
        Assert.Equal(4, error.Position);
        Assert.Equal(input.Length - 4, error.Length);
    }
}