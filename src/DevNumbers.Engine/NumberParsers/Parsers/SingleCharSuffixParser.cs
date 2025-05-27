using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Parser for single character suffix (h, b, o, d)
/// </summary>
public class SingleCharSuffixParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.SingleCharSuffix;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        if (input.Length <= 1)
            return false;

        char lastChar = char.ToLowerInvariant(input[^1]);
        string suffix = input[^1].ToString();
        string value = input[..^1];

        // Check for suffix types
        switch (lastChar)
        {
            // Hexadecimal: h or H
            case 'h':
                // Ensure not ending with "hex" or "hexadecimal"
                if (input.EndsWith("hex", StringComparison.OrdinalIgnoreCase) ||
                    input.EndsWith("hexadecimal", StringComparison.OrdinalIgnoreCase))
                    break;

                if (this.TryParseHexadecimal(value, out var hexValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Hexadecimal,
                        hexValue,
                        false,
                        new FormatInfo(FormatStyle.SingleCharSuffix, value.Any(CharExtensions.IsHexLetterUppercase))
                    );
                    return true;
                }
                break;

            // Octal: o or O
            case 'o':
                // Ensure not ending with "oct" or "octal"
                if (input.EndsWith("oct", StringComparison.OrdinalIgnoreCase) ||
                    input.EndsWith("octal", StringComparison.OrdinalIgnoreCase))
                    break;

                if (this.TryParseOctal(value, out var octValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Octal,
                        octValue,
                        false,
                        new FormatInfo(FormatStyle.SingleCharSuffix)
                    );
                    return true;
                }
                break;

            // can't use single prefix for decimal or binary, because the value is ambiguous
        }

        return false;
    }
}