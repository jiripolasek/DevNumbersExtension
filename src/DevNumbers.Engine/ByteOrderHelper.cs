// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;

namespace JPSoftworks.DevNumbers.Engine;

/// <summary>
/// Provides byte-order transformations for integer bit patterns.
/// </summary>
/// <remarks>
/// This helper deliberately supports only a single 16-bit value. Wider byte reversal,
/// word swapping, and floating-point reinterpretation have different semantics and do
/// not belong in the numerical conversion implemented here.
/// </remarks>
public static class ByteOrderHelper
{
    private const int BitLength = 16;

    /// <summary>
    /// Attempts to exchange the low and high bytes of a 16-bit integer bit pattern.
    /// </summary>
    /// <param name="value">
    /// A value representable as either a signed or unsigned 16-bit integer. Negative
    /// values use their 16-bit two's-complement bit pattern, matching the engine's
    /// existing signed-value interpretation.
    /// </param>
    /// <param name="byteSwappedValue">
    /// Receives the swapped unsigned 16-bit bit pattern when the method succeeds;
    /// otherwise receives zero.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="value"/> can be interpreted as a
    /// 16-bit value; otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryByteSwap16(BigInteger value, out BigInteger byteSwappedValue)
    {
        if (value < BigIntegerExtensions.GetMinSignedValue(BitLength)
            || value > BigIntegerExtensions.GetMaxUnsignedValue(BitLength))
        {
            byteSwappedValue = BigInteger.Zero;
            return false;
        }

        // InterpretAsUnsigned converts negative inputs to the same two's-complement
        // pattern used elsewhere by the engine. The masks then operate on exactly two
        // bytes, regardless of the input's original decimal or prefixed format.
        var bitPattern = value.InterpretAsUnsigned(BitLength);
        byteSwappedValue = ((bitPattern & 0x00FF) << 8) | ((bitPattern & 0xFF00) >> 8);
        return true;
    }
}