using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

/// <summary>
/// Parser for Visual Basic style prefixes (&amp;H, &amp;B, &amp;O)
/// </summary>
internal class VBStylePrefixParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.VBStylePrefix;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        if (input.Length <= 2 || input[0] != '&')
            return false;

        char formatChar = char.ToUpperInvariant(input[1]);
        if (formatChar != 'H' && formatChar != 'B' && formatChar != 'O')
            return false;

        string prefix = input[..2];
        string value = input[2..];

        switch (formatChar)
        {
            // Hexadecimal: &H
            case 'H':
                if (this.TryParseHexadecimal(value, out var hexValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Hexadecimal,
                        hexValue,
                        false,
                        new FormatInfo(FormatStyle.VBStylePrefix, value.Any(CharExtensions.IsHexLetterUppercase))
                    );
                    return true;
                }
                break;

            // Binary: &B
            case 'B':
                if (this.TryParseBinary(value, out var binValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Binary,
                        binValue,
                        false,
                        new FormatInfo(FormatStyle.VBStylePrefix)
                    );
                    return true;
                }
                break;

            // Octal: &O
            case 'O':
                if (this.TryParseOctal(value, out var octValue))
                {
                    result = new InputFormatParserResult(
                        input,
                        NumberBase.Octal,
                        octValue,
                        false,
                        new FormatInfo(FormatStyle.VBStylePrefix)
                    );
                    return true;
                }
                break;
        }

        return false;
    }
}