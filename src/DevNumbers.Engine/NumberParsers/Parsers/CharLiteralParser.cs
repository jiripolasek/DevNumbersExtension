using System.Globalization;
using System.Numerics;
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

        // Check for valid character literal format: 'char'
        if (input.Length < 3 || input[0] != '\'' || input[^1] != '\'')
            return false;

        // Extract the character content (excluding the quotes)
        ReadOnlySpan<char> charContent = input.AsSpan(1, input.Length - 2);

        // Handle simple character
        if (charContent.Length == 1)
        {
            BigInteger charValue = new BigInteger(charContent[0]);
            result = new InputFormatParserResult(
                input,
                NumberBase.Char,
                charValue,
                false,
                new FormatInfo(FormatStyle.CharLiteral)
            );
            return true;
        }

        // Handle escape sequences
        if (charContent.Length >= 2 && charContent[0] == '\\')
        {
            // Skip the backslash for the switch
            char escapeChar = charContent[1];
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
                'u' => this.ParseUnicodeEscape(charContent[2..], 4, out charValue) ? charValue : -1,
                'U' => this.ParseUnicodeEscape(charContent[2..], 8, out charValue) ? charValue : -1,
                'x' => this.ParseHexEscape(charContent[2..], out charValue) ? charValue : -1,

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
        }

        return false;
    }

    private bool ParseUnicodeEscape(ReadOnlySpan<char> hexDigits, int requiredLength, out int charValue)
    {
        charValue = 0;

        if (hexDigits.Length < requiredLength)
            return false;

        ReadOnlySpan<char> relevantDigits = hexDigits[..requiredLength];

        if (!relevantDigits.ToArray().All(char.IsAsciiHexDigit))
            return false;

        return int.TryParse(relevantDigits.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out charValue);
    }

    private bool ParseHexEscape(ReadOnlySpan<char> hexDigits, out int charValue)
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