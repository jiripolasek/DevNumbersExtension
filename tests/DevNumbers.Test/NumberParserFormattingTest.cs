// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Test;

/// <summary>
/// Tests for the NumberParser.Format and NumberParser.Format2 methods
/// </summary>
public class NumberParserFormattingTest
{
    [Theory]
    [MemberData(nameof(GetFormatTestData))]
    public void Format_InputValueBaseAndStyle_ReturnsCorrectlyFormattedString(
        BigInteger value, 
        NumberBase numberBase, 
        FormatStyle formatStyle, 
        bool hasUnderscoreSeparator,
        bool isUpperCase,
        string expectedOutput)
    {
        // Arrange
        var formatInfo = new FormatInfo(formatStyle, isUpperCase);
        
        // Act
        var result = NumberParser.Format(value, numberBase, formatInfo);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Theory]
    [MemberData(nameof(GetFormatExceptionsTestData))]
    public void Format_UnsupportedFormatStyle_ThrowsArgumentException(
        BigInteger value,
        NumberBase numberBase,
        FormatStyle formatStyle)
    {
        // Arrange
        var formatInfo = new FormatInfo(formatStyle);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => NumberParser.Format(value, numberBase, formatInfo));
    }
    
    [Theory]
    [MemberData(nameof(GetFormat2TestData))]
    public void Format2_InputValueBaseAndStyle_ReturnsCorrectlyFormattedString(
        BigInteger value, 
        NumberBase numberBase, 
        FormatStyle formatStyle, 
        bool hasUnderscoreSeparator,
        bool isUpperCase,
        string expectedOutput)
    {
        // Arrange
        var formatInfo = new FormatInfo(formatStyle, isUpperCase);
        
        // Act
        var result = NumberParser.FormatWithFallback(value, numberBase, formatInfo);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    [Theory]
    [MemberData(nameof(GetFormat2FallbackTestData))]
    public void Format2_UnsupportedFormatStyle_FallsBackToStandard(
        BigInteger value,
        NumberBase numberBase,
        FormatStyle formatStyle,
        string expectedOutput)
    {
        // Arrange
        var formatInfo = new FormatInfo(formatStyle);

        // Act
        var result = NumberParser.FormatWithFallback(value, numberBase, formatInfo);

        // Assert
        Assert.Equal(expectedOutput, result);
    }

    public static IEnumerable<object[]> GetFormatTestData()
    {
        // Decimal format tests
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.Standard, false, false, "42"];
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.CSharpStyle, false, false, "42"];
        yield return [new BigInteger(1000000), NumberBase.Decimal, FormatStyle.CSharpStyle, true, false, "1_000_000"];
        yield return [new BigInteger(1000000), NumberBase.Decimal, FormatStyle.CppStyle, false, false, "1'000'000"];
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.MultiCharSuffix, false, false, "42dec"];
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.AdaStylePrefix, false, false, "10#42#"];
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.RStylePrefix, false, false, "10r42"];
        
        // Hexadecimal format tests
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.CSharpStyle, false, false, "0xff"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.CSharpStyle, false, true, "0xFF"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.CStylePrefix, false, false, "0xff"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.CppStyle, false, false, "0xff"];
        yield return [new BigInteger(65535), NumberBase.Hexadecimal, FormatStyle.CppStyle, false, false, "0xffff"];
        yield return [new BigInteger(65536), NumberBase.Hexadecimal, FormatStyle.CppStyle, false, false, "0x1'0000"];
        yield return [new BigInteger(2882382797), NumberBase.Hexadecimal, FormatStyle.CppStyle, false, true, "0xABCD'ABCD"];
        yield return [new BigInteger(65536), NumberBase.Hexadecimal, FormatStyle.CSharpStyle, true, false, "0x_1_0000"];
        yield return [new BigInteger(65536), NumberBase.Hexadecimal, FormatStyle.CStylePrefix, false, false, "0x10000"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.SingleCharSuffix, false, true, "FFh"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.MultiCharSuffix, false, false, "ffhex"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.SingleCharPrefix, false, false, "hff"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.SpecialCharPrefix, false, true, "#FF"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.VBStylePrefix, false, true, "&HFF"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.AdaStylePrefix, false, false, "16#ff#"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.RStylePrefix, false, false, "16rff"];
        yield return [new BigInteger(2882382797), NumberBase.Hexadecimal, FormatStyle.CSharpStyle, true, true, "0x_ABCD_ABCD"];
        
        // Binary format tests
        yield return [new BigInteger(10), NumberBase.Binary, FormatStyle.CSharpStyle, false, false, "0b1010"];
        yield return [new BigInteger(10), NumberBase.Binary, FormatStyle.CStylePrefix, false, false, "0b1010"];
        yield return [new BigInteger(15), NumberBase.Binary, FormatStyle.CppStyle, false, false, "0b1111"];
        yield return [new BigInteger(16), NumberBase.Binary, FormatStyle.CppStyle, false, false, "0b1'0000"];
        yield return [new BigInteger(10), NumberBase.Binary, FormatStyle.MultiCharSuffix, false, false, "1010bin"];
        yield return [new BigInteger(10), NumberBase.Binary, FormatStyle.SpecialCharPrefix, false, false, "%1010"];
        yield return [new BigInteger(10), NumberBase.Binary, FormatStyle.VBStylePrefix, false, false, "&B1010"];
        yield return [new BigInteger(10), NumberBase.Binary, FormatStyle.AdaStylePrefix, false, false, "2#1010#"];
        yield return [new BigInteger(10), NumberBase.Binary, FormatStyle.RStylePrefix, false, false, "2r1010"];
        yield return [new BigInteger(170), NumberBase.Binary, FormatStyle.CSharpStyle, true, false, "0b_1010_1010"];
        
        // Octal format tests
        yield return [new BigInteger(42), NumberBase.Octal, FormatStyle.CSharpStyle, false, false, "052"];
        yield return [new BigInteger(512), NumberBase.Octal, FormatStyle.CppStyle, false, false, "01'000"];
        yield return [new BigInteger(42), NumberBase.Octal, FormatStyle.SingleCharSuffix, false, false, "52o"];
        yield return [new BigInteger(42), NumberBase.Octal, FormatStyle.MultiCharSuffix, false, false, "52oct"];
        yield return [new BigInteger(42), NumberBase.Octal, FormatStyle.SingleCharPrefix, false, false, "o52"];
        yield return [new BigInteger(42), NumberBase.Octal, FormatStyle.SpecialCharPrefix, false, false, "@52"];
        yield return [new BigInteger(42), NumberBase.Octal, FormatStyle.VBStylePrefix, false, false, "&O52"];
        yield return [new BigInteger(42), NumberBase.Octal, FormatStyle.AdaStylePrefix, false, false, "8#52#"];
        yield return [new BigInteger(42), NumberBase.Octal, FormatStyle.RStylePrefix, false, false, "8r52"];
        
        // Character format tests
        yield return [new BigInteger(65), NumberBase.Char, FormatStyle.CharLiteral, false, false, "'A'"];
        yield return [new BigInteger(10), NumberBase.Char, FormatStyle.CharLiteral, false, false, "'\\n'"];
        yield return [new BigInteger(9), NumberBase.Char, FormatStyle.CharLiteral, false, false, "'\\t'"];
        yield return [new BigInteger(92), NumberBase.Char, FormatStyle.CharLiteral, false, false, "'\\\\'"];
    }

    public static IEnumerable<object[]> GetFormatExceptionsTestData()
    {
        // Character literals only support CharLiteral style
        yield return [new BigInteger(65), NumberBase.Char, FormatStyle.Standard];
        yield return [new BigInteger(65), NumberBase.Char, FormatStyle.CSharpStyle];

        // Some formatters may not support certain bases
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.SingleCharPrefix];
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.SpecialCharPrefix];
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.VBStylePrefix];
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.SingleCharSuffix];
    }

    public static IEnumerable<object[]> GetFormat2TestData()
    {
        // Normal supported format combinations
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.Standard, false, false, "42"];
        yield return [new BigInteger(-42), NumberBase.Decimal, FormatStyle.Standard, false, false, "-42"];
        yield return [new BigInteger(65536), NumberBase.Hexadecimal, FormatStyle.CppStyle, false, false, "0x1'0000"];
        yield return [new BigInteger(255), NumberBase.Hexadecimal, FormatStyle.CSharpStyle, false, false, "0xff"];
        yield return [new BigInteger(10), NumberBase.Binary, FormatStyle.CSharpStyle, false, false, "0b1010"];
    }

    public static IEnumerable<object[]> GetFormat2FallbackTestData()
    {
        // Character literals trying to use other styles should fall back to standard
        yield return [new BigInteger(65), NumberBase.Char, FormatStyle.Standard, "65"];

        // Unsupported format style combinations that should fall back to standard
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.SingleCharPrefix, "42"];
        yield return [new BigInteger(42), NumberBase.Decimal, FormatStyle.SpecialCharPrefix, "42"];
    }
}