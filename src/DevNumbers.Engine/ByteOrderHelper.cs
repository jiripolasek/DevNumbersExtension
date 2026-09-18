// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;

namespace JPSoftworks.DevNumbers.Engine;

/// <summary>
/// Swaps bytes in 16-bit integer bit patterns.
/// </summary>
public static class ByteOrderHelper
{
    private const int BitLength = 16;

    /// <summary>
    /// Exchanges the low and high bytes when <paramref name="value"/> fits a 16-bit pattern.
    /// </summary>
    /// <param name="value">A signed or unsigned 16-bit value.</param>
    /// <param name="byteSwappedValue">The swapped unsigned bit pattern, or zero on failure.</param>
    /// <returns>Whether <paramref name="value"/> fits the supported range.</returns>
    public static bool TryByteSwap16(BigInteger value, out BigInteger byteSwappedValue)
    {
        if (value < BigIntegerExtensions.GetMinSignedValue(BitLength)
            || value > BigIntegerExtensions.GetMaxUnsignedValue(BitLength))
        {
            byteSwappedValue = BigInteger.Zero;
            return false;
        }

        // Match the engine's existing two's-complement handling for negative inputs.
        var bitPattern = value.InterpretAsUnsigned(BitLength);
        byteSwappedValue = ((bitPattern & 0x00FF) << 8) | ((bitPattern & 0xFF00) >> 8);
        return true;
    }
}
