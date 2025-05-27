// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;

namespace JPSoftworks.DevNumbers.Engine;

public static class BigIntegerExtensions
{
    /// <summary>
    /// Trims a BigInteger to the specified bit length by keeping only the lower bits.
    /// This is equivalent to AsUnsigned but with a more descriptive name for truncation operations.
    /// </summary>
    /// <param name="value">The BigInteger value to trim</param>
    /// <param name="bitLength">The number of bits to keep</param>
    /// <returns>The trimmed value containing only the lower bitLength bits</returns>
    public static BigInteger TrimToBitLength(this BigInteger value, int bitLength)
    {
        if (bitLength <= 0)
            throw new ArgumentException("Bit length must be positive", nameof(bitLength));

        // Optimization: If the value is already within the bit length, return as-is
        if (value.Sign >= 0 && value.GetBitLength() <= bitLength)
        {
            return value;
        }
        BigInteger mask = (BigInteger.One << bitLength) - 1;
        return value & mask;
    }

    /// <summary>
    /// Gets the bit length of the BigInteger value, including the sign bit for negative values.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetBitLengthWithSign(this BigInteger value)
    {
        return (int)value.GetBitLength() + (value < 0 ? 1 : 0);
    }

    /// <summary>
    /// Get size of the container in the power of 2 increments that can hold the value in bits.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="signed"></param>
    /// <returns></returns>
    public static int GetNearestContainerBitLength(this BigInteger value, bool signed)
    {
        var bits = value.GetBitLength() + (signed ? 1 : 0);
        return (int)NextPowerOfTwo(bits);
    }

    /// <summary>
    /// Get size of the container in the power of 2 increments that can hold the value in bits.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int GetNearestContainerBitLength(this BigInteger value)
    {
        return (int)NextPowerOfTwo(value.GetBitLengthWithSign());
    }

    public static BigInteger GetTwoComplement(this BigInteger value, int bitLength)
    {
        return value.GetBitLength() < bitLength
            ? value
            : (BigInteger.One << bitLength) - value;
    }


    public static BigInteger InterpretAsSigned(this BigInteger value, int bitLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bitLength);

        var unsignedValue = InterpretAsUnsigned(value, bitLength);
        var signBit = BigInteger.One << (bitLength - 1);

        if ((unsignedValue & signBit) != 0)
        {
            BigInteger maxValue = BigInteger.One << bitLength;
            return unsignedValue - maxValue;
        }
        return unsignedValue;
    }

    public static BigInteger InterpretAsUnsigned(this BigInteger value, int bitLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bitLength);
        var mask = (BigInteger.One << bitLength) - 1;
        return value & mask;
    }

    /// <summary>
    /// Gets the minimum signed value that can be represented with the specified bit length.
    /// </summary>
    /// <param name="bitLength">The number of bits</param>
    /// <returns>The minimum signed value (-2^(bitLength-1))</returns>
    public static BigInteger GetMinSignedValue(int bitLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bitLength);
        return -(BigInteger.One << (bitLength - 1));
    }

    /// <summary>
    /// Gets the maximum signed value that can be represented with the specified bit length.
    /// </summary>
    /// <param name="bitLength">The number of bits</param>
    /// <returns>The maximum signed value (2^(bitLength-1) - 1)</returns>
    public static BigInteger GetMaxSignedValue(int bitLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bitLength);
        return (BigInteger.One << (bitLength - 1)) - 1;
    }

    /// <summary>
    /// Gets the maximum unsigned value that can be represented with the specified bit length.
    /// </summary>
    /// <param name="bitLength">The number of bits</param>
    /// <returns>The maximum unsigned value (2^bitLength - 1)</returns>
    public static BigInteger GetMaxUnsignedValue(int bitLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bitLength);
        return (BigInteger.One << bitLength) - 1;
    }

    private static long NextPowerOfTwo(long bits)
    {
        var power = 8;
        while (power < bits)
        {
            power <<= 1;
        }

        return power;
    }

}