using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Parser for R-style base notation (16rABCD, 2r1010, 8r777, 10r1234)
/// </summary>
internal class RStylePrefixParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.RStylePrefix;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        // Look for R-style base prefix: baservalue
        int rIndex = input.IndexOf('r', StringComparison.OrdinalIgnoreCase);
        if (rIndex <= 0) return false;

        string baseStr = input[..rIndex];
        string valueStr = input[(rIndex + 1)..];
        string rChar = input[rIndex].ToString(); // Get the actual case of 'r'

        // Skip if empty value
        if (string.IsNullOrEmpty(valueStr)) return false;

        // Try to parse the base
        if (!int.TryParse(baseStr, out int baseValue)) return false;

        string prefix = $"{baseStr}{rChar}";

        // Handle different bases
        switch (baseValue)
        {
            case 2: // Binary
                if (this.TryParseBinary(valueStr, out var binValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Binary,
                        binValue,
                        false,
                        new FormatInfo(FormatStyle.RStylePrefix)
                    );
                    return true;
                }
                break;

            case 8: // Octal
                if (this.TryParseOctal(valueStr, out var octValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Octal,
                        octValue,
                        false,
                        new FormatInfo(FormatStyle.RStylePrefix)
                    );
                    return true;
                }
                break;

            case 10: // Decimal
                if (this.TryParseDecimal(valueStr, out var decValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Decimal,
                        decValue,
                        false,
                        new FormatInfo(FormatStyle.RStylePrefix)
                    );
                    return true;
                }
                break;

            case 16: // Hexadecimal
                if (this.TryParseHexadecimal(valueStr, out var hexValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Hexadecimal,
                        hexValue,
                        false,
                        new FormatInfo(FormatStyle.RStylePrefix, valueStr.Any(CharExtensions.IsHexLetterUppercase))
                    );
                    return true;
                }
                break;
        }

        return false;
    }
}