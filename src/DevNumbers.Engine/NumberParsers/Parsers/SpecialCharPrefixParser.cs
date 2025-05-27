using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Parser for special character prefix (#, $, %, @)
/// </summary>
internal class SpecialCharPrefixParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.SpecialCharPrefix;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        if (input.Length <= 1)
            return false;

        char firstChar = input[0];
        string prefix = firstChar.ToString();
        string value = input[1..];

        // Check for special character prefixes
        switch (firstChar)
        {
            // Hexadecimal: # (CSS/HTML style) or $ (Assembly style)
            case '#':
            case '$':
                if (this.TryParseHexadecimal(value, out var hexValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Hexadecimal,
                        hexValue,
                        false,
                        new FormatInfo(FormatStyle.SpecialCharPrefix, value.Any(CharExtensions.IsHexLetterUppercase))
                    );
                    return true;
                }
                break;

            // Binary: % (Assembly style)
            case '%':
                if (this.TryParseBinary(value, out var binValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Binary,
                        binValue,
                        false,
                        new FormatInfo(FormatStyle.SpecialCharPrefix)
                    );
                    return true;
                }
                break;

            // Octal: @ 
            case '@':
                if (this.TryParseOctal(value, out var octValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Octal,
                        octValue,
                        false,
                        new FormatInfo(FormatStyle.SpecialCharPrefix)
                    );
                    return true;
                }
                break;
        }

        return false;
    }
}