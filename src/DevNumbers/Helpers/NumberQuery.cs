// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;
using JPSoftworks.DevNumbers.Engine;
using JPSoftworks.DevNumbers.Engine.NumberParsers;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Helpers;

internal sealed record NumberQuery(
    InputFormatParserResult Number,
    BigInteger Value,
    int BitLength,
    FormatStyle FormatStyle)
{
    public static NumberQuery Parse(ParseResult query, FormatStyle? defaultFormatStyle)
    {
        var number = NumberParser.Parse(query.Query);
        if (number is null || number.NumberBase == NumberBase.Unknown)
        {
            throw new FormatException("Unrecognized number format.");
        }

        var bitLength = query.Options.BitLength;
        var value = number.Value;

        // a) If a bit length is specified, apply it to the parsed input:
        //    values that do not fit are truncated to that bit length.
        // b) If no bit length is specified:
        //    - for negative values, automatically determine the best bit length;
        //    - for positive values, leave the bit length unspecified.
        if (bitLength > 0)
        {
            value = value.TrimToBitLength(bitLength);
        }
        else if (value < 0)
        {
            bitLength = value.GetNearestContainerBitLength(true);
        }

        return new NumberQuery(number, value, bitLength, defaultFormatStyle ?? number.FormatInfo.Style);
    }
}