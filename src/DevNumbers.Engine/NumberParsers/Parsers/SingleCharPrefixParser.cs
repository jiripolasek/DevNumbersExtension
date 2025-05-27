using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Parser for single character prefix (h, x, b, o, d)
/// </summary>
internal class SingleCharPrefixParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.SingleCharPrefix;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        if (input.Length <= 1)
            return false;

        char firstChar = char.ToLowerInvariant(input[0]);
        string prefix = input[0].ToString();
        string value = input[1..];

        // Check for prefix types
        switch (firstChar)
        {
            // Hexadecimal: h, H, x, X
            case 'h':
            case 'x':
                if (this.TryParseHexadecimal(value, out var hexValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Hexadecimal,
                        hexValue,
                        false,
                        new FormatInfo(FormatStyle.SingleCharPrefix, value.Any(CharExtensions.IsHexLetterUppercase))
                    );
                    return true;
                }
                break;

            // Octal: o or O
            case 'o':
                if (this.TryParseOctal(value, out var octValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Octal,
                        octValue,
                        false,
                        new FormatInfo(FormatStyle.SingleCharPrefix)
                    );
                    return true;
                }
                break;

            // Decimal (D,d) or binary (B,b) can't be used as prefixes, because the value is ambiguous
        }

        return false;
    }
}