// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;
using JPSoftworks.DevNumbers.Engine;

namespace JPSoftworks.DevNumbers.Test;

public class ByteOrderHelperTest
{
    [Theory]
    [InlineData(0, 0x0000)]
    [InlineData(1, 0x0100)]
    [InlineData(2, 0x0200)]
    [InlineData(512, 0x0002)]
    [InlineData(0x1234, 0x3412)]
    [InlineData(0xABCD, 0xCDAB)]
    [InlineData(0xFFFF, 0xFFFF)]
    public void TryByteSwap16_ValidBitPattern_ReturnsSwappedValue(int input, int expected)
    {
        // Act
        var success = ByteOrderHelper.TryByteSwap16(new BigInteger(input), out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(new BigInteger(expected), result);
    }

    [Fact]
    public void TryByteSwap16_NegativeValue_SwapsTwosComplementBitPattern()
    {
        // Arrange: -2 is 0xFFFE when interpreted as a 16-bit unsigned pattern.
        var input = new BigInteger(-2);

        // Act
        var success = ByteOrderHelper.TryByteSwap16(input, out var result);

        // Assert: swapping 0xFFFE produces the unsigned 16-bit pattern 0xFEFF.
        Assert.True(success);
        Assert.Equal(new BigInteger(0xFEFF), result);
    }

    [Fact]
    public void TryByteSwap16_ValueTrimmedToExplicit16Bits_SwapsTruncatedPattern()
    {
        // Arrange: explicit /length:16 handling turns -65538 into the low 16 bits, 0xFFFE.
        var valueAfterExplicitBitLength = new BigInteger(-65538).TrimToBitLength(16);

        // Act
        var success = ByteOrderHelper.TryByteSwap16(valueAfterExplicitBitLength, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(new BigInteger(0xFEFF), result);
    }

    [Theory]
    [InlineData(-32769)]
    [InlineData(65536)]
    public void TryByteSwap16_ValueOutside16BitRange_ReturnsFalse(int input)
    {
        // Act
        var success = ByteOrderHelper.TryByteSwap16(new BigInteger(input), out var result);

        // Assert
        Assert.False(success);
        Assert.Equal(BigInteger.Zero, result);
    }
}
