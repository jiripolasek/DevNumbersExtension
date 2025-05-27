using System.Numerics;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Parser for multi-character suffix (bin, oct, hex, dec)
/// </summary>
internal class MultiCharSuffixParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.MultiCharSuffix;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        // Try to match different suffix patterns
        foreach (var (suffix, format, parser) in this.GetSuffixPatterns())
        {
            if (input.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                // Get the value portion and the exact suffix case
                string value = input[..^suffix.Length];
                string exactSuffix = input[^suffix.Length..];

                // Parse the value according to the format
                bool success = false;
                BigInteger parsedValue = BigInteger.Zero;

                switch (parser)
                {
                    case "hex":
                        success = this.TryParseHexadecimal(value, out parsedValue);
                        break;
                    case "bin":
                        success = this.TryParseBinary(value, out parsedValue);
                        break;
                    case "oct":
                        success = this.TryParseOctal(value, out parsedValue);
                        break;
                    case "dec":
                        success = this.TryParseDecimal(value, out parsedValue);
                        break;
                }

                if (success)
                {
                    result = new InputFormatParserResult(
                        input,
                        format,
                        parsedValue,
                        false,
                        new FormatInfo(
                            FormatStyle.MultiCharSuffix,
                            format == NumberBase.Hexadecimal && value.Any(CharExtensions.IsHexLetterUppercase)
                        )
                    );
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerable<(string Suffix, NumberBase Format, string Parser)> GetSuffixPatterns()
    {
        // Hexadecimal
        yield return ("hex", NumberBase.Hexadecimal, "hex");
        yield return ("hexadecimal", NumberBase.Hexadecimal, "hex");

        // Binary
        yield return ("bin", NumberBase.Binary, "bin");
        yield return ("binary", NumberBase.Binary, "bin");

        // Octal
        yield return ("oct", NumberBase.Octal, "oct");
        yield return ("octal", NumberBase.Octal, "oct");

        // Decimal
        yield return ("dec", NumberBase.Decimal, "dec");
        yield return ("decimal", NumberBase.Decimal, "dec");
    }
}