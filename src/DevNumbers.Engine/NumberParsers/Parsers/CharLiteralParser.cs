// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using System.Globalization;
using System.Numerics;
using System.Linq;
using System.Text;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Parser for C# character literals, following the C# language specification
/// </summary>
internal class CharLiteralParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.CharLiteral;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        if (input.Length < 3 || input[0] != '\'' || input[^1] != '\'')
            return false;

        var charContent = input.AsSpan(1, input.Length - 2);
        
        switch (charContent.Length)
        {
            case 1:
                {
                    var charValue = new BigInteger(charContent[0]);
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Char,
                        charValue,
                        false,
                        new FormatInfo(FormatStyle.CharLiteral)
                    );
                    return true;
                }

            case >= 2 when charContent[0] == '\\':
                {
                    // Skip the backslash for the switch
                    var escapeChar = charContent[1];
                    if (charContent.Length != 2 && escapeChar is not ('u' or 'U' or 'x'))
                        return false;

                    int charValue;

                    // Handle standard escape sequences
                    charValue = escapeChar switch
                    {
                        '\'' => '\'',  // Single quote
                        '"' => '"',    // Double quote
                        '\\' => '\\',  // Backslash
                        '0' => '\0',   // Null
                        'a' => '\a',   // Alert (bell)
                        'b' => '\b',   // Backspace
                        'f' => '\f',   // Form feed
                        'n' => '\n',   // New line
                        'r' => '\r',   // Carriage return
                        't' => '\t',   // Horizontal tab
                        'v' => '\v',   // Vertical tab

                        // Handle Unicode and hex escape sequences
                        'u' => ParseUnicodeEscape(charContent[2..], 4, out charValue) ? charValue : -1, // C# Unicode escape
                        'U' => ParseUnicodeEscape(charContent[2..], 8, out charValue) ? charValue : -1, // Python-style Unicode escape
                        'x' => ParseHexEscape(charContent[2..], out charValue) ? charValue : -1, // Hex escape sequence

                        _ => -1 // Invalid escape sequence
                    };

                    if (charValue != -1)
                    {
                        result = new InputFormatParserResult(
                            input,
                            NumberBase.Char,
                            new BigInteger(charValue),
                            false,
                            new FormatInfo(FormatStyle.CharLiteral)
                        );
                        return true;
                    }

                    break;
                }

            case >= 6 when charContent.StartsWith("U+"): // Unicode codepoint
            case >= 6 when charContent.StartsWith("U-"): // historical notation
                {
                    var hexDigits = charContent[2..];
                    if (ParseVariableUnicodeEscape(hexDigits, 4, out var charValue))
                    {
                        result = new InputFormatParserResult(
                            input,
                            NumberBase.Char,
                            new BigInteger(charValue),
                            false,
                            new FormatInfo(FormatStyle.CharLiteral)
                        );
                        return true;
                    }

                    break;
                }
        }

        if (Rune.TryGetRuneAt(input, 1, out var rune) && rune.Utf16SequenceLength == charContent.Length)
        {
            if (rune.Value is < 0xD800 or > 0xDFFF) // Exclude surrogate pairs
            {
                result = new InputFormatParserResult(
                    input,
                    NumberBase.Char,
                    new BigInteger(rune.Value),
                    false,
                    new FormatInfo(FormatStyle.CharLiteral)
                );
                return true;
            }
        }

        return false;
    }

    private static bool ParseUnicodeEscape(ReadOnlySpan<char> hexDigits, int requiredLength, out int charValue)
    {
        charValue = 0;

        if (hexDigits.Length != requiredLength)
            return false;

        if (!hexDigits.ToArray().All(char.IsAsciiHexDigit))
            return false;

        return int.TryParse(hexDigits.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out charValue);
    }

    private static bool ParseVariableUnicodeEscape(ReadOnlySpan<char> hexDigits, int requiredMinLength, out int charValue)
    {
        charValue = 0;

        if (hexDigits.Length < requiredMinLength)
            return false;

        if (!hexDigits.ToArray().All(char.IsAsciiHexDigit))
            return false;

        return int.TryParse(hexDigits.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out charValue);
    }

    private static bool ParseHexEscape(ReadOnlySpan<char> hexDigits, out int charValue)
    {
        charValue = 0;

        // \x can be followed by 1-4 hex digits
        if (hexDigits.IsEmpty || hexDigits.Length > 4)
            return false;

        // All characters must be hex digits
        if (!hexDigits.ToArray().All(char.IsAsciiHexDigit))
            return false;

        return int.TryParse(hexDigits.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out charValue);
    }
}