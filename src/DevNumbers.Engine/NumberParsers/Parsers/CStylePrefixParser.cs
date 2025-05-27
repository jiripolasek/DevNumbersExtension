// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Parser for C-style prefix (0x, 0b, 0)
/// </summary>
public abstract class CStylePrefixParser : NumberParserBase
{
    private readonly char _groupSeparator;
    public override FormatStyle SupportedStyle => FormatStyle.CStylePrefix;

    protected CStylePrefixParser(char groupSeparator)
    {
        this._groupSeparator = groupSeparator;
    }

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        if (input.Length <= 1)
            return false;

        // if input is null, empty, whitespace or contains only group separators, return false
        if (string.IsNullOrWhiteSpace(input) || input.All(c => c == this._groupSeparator))
            return false;

        // Check for 0x, 0X, 0h, 0H prefixes (hexadecimal)
        if (input.Length > 2 && (
                input.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ||
                input.StartsWith("0h", StringComparison.OrdinalIgnoreCase)))
        {
            string prefix = input[..2];
            string value = input[2..];


            if (BigIntegerBaseConverter.TryParse(value, BigIntegerBaseStyle.HexadecimalUppercase with { GroupSeparator = this._groupSeparator, AllowGroupSeparator = true }, out var hexValue))
            {
                result = new InputFormatParserResult(
                    input,
                    NumberBase.Hexadecimal,
                    hexValue,
                    false,
                    new FormatInfo(this.SupportedStyle, value.Any(CharExtensions.IsHexLetterUppercase))
                );
                return true;
            }
        }

        // Check for 0b, 0B prefixes (binary)
        if (input.Length > 2 && input.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
        {
            string prefix = input[..2];
            string value = input[2..];

            if (BigIntegerBaseConverter.TryParse(value, BigIntegerBaseStyle.Binary with { GroupSeparator = this._groupSeparator, AllowGroupSeparator = true }, out var binValue))
            {
                result = new InputFormatParserResult(
                    input,
                    NumberBase.Binary,
                    binValue,
                    false,
                    new FormatInfo(this.SupportedStyle)
                );
                return true;
            }
        }

        // Check for leading 0 prefix (octal)
        if (input[0] == '0')
        {
            // Ensure not 0x, 0b, or 0h
            if (input.Length > 1 && (
                    input[1] == 'x' || input[1] == 'X' ||
                    input[1] == 'b' || input[1] == 'B' ||
                    input[1] == 'h' || input[1] == 'H'))
                return false;

            string value = input[1..];

            // Must be all octal digits
            if (BigIntegerBaseConverter.TryParse(value, BigIntegerBaseStyle.Octal with { GroupSeparator = this._groupSeparator, AllowGroupSeparator = true }, out var octValue))
            {
                result = new InputFormatParserResult(
                    input,
                    NumberBase.Octal,
                    octValue,
                    false,
                    new FormatInfo(this.SupportedStyle)
                );
                return true;
            }
        }

        // Check for decimal (no prefix)
        // If input contains group separator, then try to remove it and parse as decimal
        // If there are no group separators, let just fall through to the standard parser.
        if (input[0] != '0'
            && input.Any(c => c == this._groupSeparator)
            && BigIntegerBaseConverter.TryParse(input, BigIntegerBaseStyle.Decimal with { GroupSeparator = this._groupSeparator, AllowGroupSeparator = true }, out var decValue))
        {
            result = new InputFormatParserResult(
                input,
                NumberBase.Decimal,
                decValue,
                false,
                new FormatInfo(this.SupportedStyle)
            );
            return true;
        }

        return false;
    }
}