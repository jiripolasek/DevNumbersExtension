// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace JPSoftworks.DevNumbers.Engine;


public static class BigIntegerBaseConverter
{
    public static string ToString(BigInteger value, BigIntegerBaseStyle style)
    {
        CheckStyle(style);

        if (value.IsZero)
        {
            return "0";
        }

        var isNegative = value.Sign < 0;

        BigInteger displayValue = value;
        if (value.Sign < 0)
        {
            if (style.Radix != 10)
            {
                var sizeHintBits = style.SizeHintBits ?? value.GetNearestContainerBitLength();
                var mask = (BigInteger.One << sizeHintBits) - 1;
                displayValue = value & mask;
            }
            else
            {
                displayValue = -value;
            }
        }

        var sb = new StringBuilder();
        var digits = style.UpperCase ? "0123456789ABCDEF" : "0123456789abcdef";
        int groupSize = style.GroupSize;
        int groupCounter = 0;

        while (displayValue > 0)
        {
            var rem = (int)(displayValue % style.Radix);
            sb.Insert(0, digits[rem]);
            displayValue /= style.Radix;
            groupCounter++;

            if (groupSize > 0 && displayValue > 0 && groupCounter % groupSize == 0 && style.AllowGroupSeparator)
            {
                sb.Insert(0, style.GroupSeparator);
            }
        }

        if (isNegative && style.Radix == 10)
        {
            sb.Insert(0, '-');
        }

        return sb.ToString();
    }

    public static BigInteger Parse(string input, BigIntegerBaseStyle style)
    {
        CheckStyle(style);

        input = input.Trim();

        var startsWithMinus = input.StartsWith("-", StringComparison.InvariantCultureIgnoreCase);
        if (startsWithMinus && style.Radix != 10)
        {
            throw new FormatException("Only decimal numbers can be negative");
        }

        if (startsWithMinus)
        {
            input = input[1..];
        }

        // Remove group separators
        if (style.AllowGroupSeparator)
        {
            input = input.Replace(style.GroupSeparator.ToString(), "");
        }
        else if (input.Contains('_'))
        {
            throw new FormatException("Group separator '_' is not allowed.");
        }

        BigInteger value = BigInteger.Zero;
        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                continue;
            }

            int digit = c switch
            {
                >= '0' and <= '9' => c - '0',
                >= 'a' and <= 'f' => 10 + (c - 'a'),
                >= 'A' and <= 'F' => 10 + (c - 'A'),
                _ => throw new FormatException($"Invalid character '{c}'.")
            };
            if (digit >= style.Radix)
            {
                throw new FormatException($"Invalid digit '{c}' for base {style.Radix}.");
            }

            value = value * style.Radix + digit;
        }

        if (style.Radix == 10)
        {
            return startsWithMinus ? -value : value;
        }

        return value;
    }

    public static bool TryParse(string input, BigIntegerBaseStyle style, out BigInteger value)
    {
        try
        {
            value = Parse(input, style);
            return true;
        }
        catch
        {
            value = BigInteger.Zero;
            return false;
        }
    }

    private static void CheckStyle(BigIntegerBaseStyle style, [CallerArgumentExpression(nameof(style))] string? paramName = null)
    {
        if (style is null)
        {
            throw new ArgumentNullException(paramName ?? nameof(style));
        }

        if (style.Radix is not (2 or 8 or 10 or 16))
        {
            throw new ArgumentOutOfRangeException(nameof(style.Radix), "Base must be 2, 8, 10, or 16.");
        }
    }
}