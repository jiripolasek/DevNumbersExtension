// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;
using JPSoftworks.DevNumbers.Engine;

namespace JPSoftworks.DevNumbers.Test;

public partial class BigIntegerBaseConverterTest
{
    [Theory]
    [InlineData("", 16, ' ')]
    [InlineData(" \t ", 2, ' ')]
    [InlineData("__", 16, '_')]
    [InlineData("''", 8, '\'')]
    [InlineData("_ \t _", 2, '_')]
    [InlineData("-", 10, ' ')]
    [InlineData("-__", 10, '_')]
    [InlineData("- \t ", 10, ' ')]
    public void Parse_WithoutDigits_ThrowsFormatException(string input, int radix, char separator)
    {
        var style = new BigIntegerBaseStyle { Radix = radix, GroupSeparator = separator };

        Assert.Throws<FormatException>(() => BigIntegerBaseConverter.Parse(input, style));
        Assert.False(BigIntegerBaseConverter.TryParse(input, style, out var value));
        Assert.Equal(BigInteger.Zero, value);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(42, "42")]
    [InlineData(-42, "-42")]
    [InlineData(1234567890, "1234567890")]
    [InlineData(-1234567890, "-1234567890")]
    public void ToString_DecimalDefault_ReturnsCorrectString(long value, string expected)
    {
        // Arrange
        var bigInteger = new BigInteger(value);
        var style = BigIntegerBaseStyle.Default;

        // Act
        var result = BigIntegerBaseConverter.ToString(bigInteger, style);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(42, "2a")]
    [InlineData(255, "ff")]
    [InlineData(65535, "ffff")]
    [InlineData(11259375, "ab cdef")]
    public void ToString_HexadecimalLowercase_ReturnsCorrectString(long value, string expected)
    {
        // Arrange
        var bigInteger = new BigInteger(value);
        var style = BigIntegerBaseStyle.Hexadecimal;

        // Act
        var result = BigIntegerBaseConverter.ToString(bigInteger, style);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(42, "2A")]
    [InlineData(255, "FF")]
    [InlineData(65535, "FFFF")]
    [InlineData(11259375, "AB CDEF")]
    public void ToString_HexadecimalUppercase_ReturnsCorrectString(long value, string expected)
    {
        // Arrange
        var bigInteger = new BigInteger(value);
        var style = BigIntegerBaseStyle.HexadecimalUppercase;

        // Act
        var result = BigIntegerBaseConverter.ToString(bigInteger, style);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(10, "1010")]
    [InlineData(42, "101010")]
    [InlineData(255, "11111111")]
    [InlineData(1023, "1111111111")]
    public void ToString_Binary_Ungrouped_ReturnsCorrectString(long value, string expected)
    {
        // Arrange
        var bigInteger = new BigInteger(value);
        var style = BigIntegerBaseStyle.Binary with { AllowGroupSeparator = false };

        // Act
        var result = BigIntegerBaseConverter.ToString(bigInteger, style);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(10, "1010")]
    [InlineData(42, "10 1010")]
    [InlineData(255, "1111 1111")]
    [InlineData(1023, "11 1111 1111")]
    public void ToString_Binary_Grouped_ReturnsCorrectString(long value, string expected)
    {
        // Arrange
        var bigInteger = new BigInteger(value);
        var style = BigIntegerBaseStyle.Binary;

        // Act
        var result = BigIntegerBaseConverter.ToString(bigInteger, style);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(8, "10")]
    [InlineData(42, "52")]
    [InlineData(63, "77")]
    [InlineData(511, "777")]
    public void ToString_Octal_ReturnsCorrectString(long value, string expected)
    {
        // Arrange
        var bigInteger = new BigInteger(value);
        var style = BigIntegerBaseStyle.Octal;

        // Act
        var result = BigIntegerBaseConverter.ToString(bigInteger, style);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToString_WithGroupSeparatorsDecimal_InsertsCorrectSeparators()
    {
        // Arrange
        var value = new BigInteger(1234567890);
        var style = BigIntegerBaseStyle.Decimal with { AllowGroupSeparator = true, GroupSeparator = '_' };

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal("1_234_567_890", result);
    }

    [Fact]
    public void ToString_WithGroupSeparatorsHex_InsertsCorrectSeparators()
    {
        // Arrange
        var value = new BigInteger(0xABCDEF);
        var style = BigIntegerBaseStyle.Hexadecimal with { AllowGroupSeparator = true, GroupSeparator = '_' };

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal("ab_cdef", result);
    }

    [Fact]
    public void ToString_WithGroupSeparatorsBinary_InsertsCorrectSeparators()
    {
        // Arrange
        var value = new BigInteger(0b10101010_10101010);
        var style = BigIntegerBaseStyle.Binary with { AllowGroupSeparator = true, GroupSeparator = '_' };

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal("1010_1010_1010_1010", result);
    }

    [Fact]
    public void ToString_WithGroupSeparatorsOctal_InsertsCorrectSeparators()
    {
        // Arrange
        var value = new BigInteger(Convert.ToInt64("12345670", 8));
        var style = BigIntegerBaseStyle.Octal with { AllowGroupSeparator = true, GroupSeparator = '_' };

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal("12_345_670", result);
    }

    [Fact]
    public void ToString_WithCustomGroupSeparator_UsesCustomSeparator()
    {
        // Arrange
        var value = new BigInteger(1234567890);
        var style = BigIntegerBaseStyle.Decimal with { AllowGroupSeparator = true, GroupSeparator = ',' };

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal("1,234,567,890", result);
    }

    [Fact]
    public void ToString_WithZeroGroupSize_DoesNotInsertSeparators()
    {
        // Arrange
        var value = new BigInteger(1234567890);
        var style = BigIntegerBaseStyle.Decimal with { AllowGroupSeparator = true, GroupSize = 0 };

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal("1234567890", result);
    }

    [Fact]
    public void ToString_WithoutAllowGroupSeparator_DoesNotInsertSeparators()
    {
        // Arrange
        var value = new BigInteger(1234567890);
        var style = BigIntegerBaseStyle.Decimal with { AllowGroupSeparator = false };

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal("1234567890", result);
    }

    [Fact]
    public void ToString_WithNegativeValue_HandlesNegativeValuesForDecimal()
    {
        // Arrange
        var value = new BigInteger(-1234567890);
        var style = BigIntegerBaseStyle.Decimal with { AllowGroupSeparator = true, GroupSeparator = '_' };

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal("-1_234_567_890", result);
    }

    [Fact]
    public void ToString_WithNegativeValue_HandlesNegativeValuesForNonDecimal()
    {
        // Arrange - For non-decimal bases, negative values are handled with bit masking
        var value = new BigInteger(-15); // In hex will be represented by the last bits
        var style = BigIntegerBaseStyle.Hexadecimal with { SizeHintBits = 8 }; // 8-bit representation

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert - -15 in 8-bit two's complement is 0xF1
        Assert.Equal("f1", result);
    }

    [Fact]
    public void ToString_WithLargeValue_HandlesLargeNumbers()
    {
        // Arrange
        var value = BigInteger.Pow(2, 100); // 2^100
        var style = BigIntegerBaseStyle.Default;

        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal("1267650600228229401496703205376", result);
    }

    [Theory]
    [MemberData(nameof(GetCustomStyles))]
    public void ToString_WithCustomStyle_ReturnsCorrectly(BigIntegerBaseStyle style, BigInteger value, string expected)
    {
        // Act
        var result = BigIntegerBaseConverter.ToString(value, style);

        // Assert
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> GetCustomStyles()
    {
        // Test custom binary style
        yield return [
            BigIntegerBaseStyle.Binary with { GroupSize = 4, AllowGroupSeparator = true, GroupSeparator = '_'},
            new BigInteger(0b10101010_10101010),
            "1010_1010_1010_1010"
        ];

        // Test custom hexadecimal style
        yield return [
            BigIntegerBaseStyle.Hexadecimal with { GroupSize = 2, AllowGroupSeparator = true, GroupSeparator = '_' },
            new BigInteger(0xABCDEF),
            "ab_cd_ef"
        ];

        // Test custom decimal style
        yield return [
            BigIntegerBaseStyle.Decimal with { GroupSize = 4, AllowGroupSeparator = true, GroupSeparator = ' ' },
            new BigInteger(1234567890),
            "12 3456 7890"
        ];
    }
}
