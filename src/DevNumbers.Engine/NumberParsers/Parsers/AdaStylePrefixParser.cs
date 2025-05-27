using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Parser for Ada-style base notation (16#ABCD#, 2#1010#, 8#777#, 10#1234#)
/// </summary>
internal class AdaStylePrefixParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.AdaStylePrefix;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        // Look for Ada-style base prefix: base#number
        int firstHash = input.IndexOf('#');
        if (firstHash <= 0) return false;

        // If there's a closing hash, use that
        int secondHash = input.IndexOf('#', firstHash + 1);
        string baseStr = input[..firstHash];
        string valueStr = secondHash > 0
            ? input[(firstHash + 1)..secondHash]
            : input[(firstHash + 1)..];

        // Skip if empty value
        if (string.IsNullOrEmpty(valueStr)) return false;

        // Try to parse the base
        if (!int.TryParse(baseStr, out int baseValue)) return false;

        string prefix = $"{baseStr}#";

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
                        new FormatInfo(FormatStyle.AdaStylePrefix)
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
                        new FormatInfo(FormatStyle.AdaStylePrefix)
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
                        new FormatInfo(FormatStyle.AdaStylePrefix)
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
                        new FormatInfo(FormatStyle.AdaStylePrefix, valueStr.Any(CharExtensions.IsHexLetterUppercase))
                    );
                    return true;
                }
                break;
        }

        return false;
    }
}