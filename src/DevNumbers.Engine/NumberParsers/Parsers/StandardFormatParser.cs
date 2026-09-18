// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;

namespace JPSoftworks.DevNumbers.Engine.NumberParsers.Parsers;

public class SquirrelStandardFormatParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.Standard;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        // this parser recognizes only decimal numbers starting with a non-zero digit
        // - this parser
        // - other parsers
        // - StandardFormatParser

        result = null;

        if (input.All(char.IsAsciiDigit) && input[0] != '0')
        {

            if (this.TryParseDecimal(input, out var decValue))
            {
                // Check if it could be binary or octal
                bool couldBeBinary = input.All(CharExtensions.IsBinaryDigit);
                bool couldBeOctal = input.All(CharExtensions.IsOctalDigit);
                bool isImplicit = couldBeBinary || couldBeOctal;

                result = new InputFormatParserResult(
                    input,
                    NumberBase.Decimal,
                    decValue,
                    isImplicit,
                    new FormatInfo(FormatStyle.Standard)
                );
                return true;
            }
        }

        return false;

    }
}

/// <summary>
/// Parser for standard format with no prefix or suffix
/// </summary>
public class StandardFormatParser : NumberParserBase
{
    public override FormatStyle SupportedStyle => FormatStyle.Standard;

    public override bool TryParse(string input, out InputFormatParserResult? result)
    {
        result = null;

        // First check for implicit hexadecimal (contains A-F)
        // Reserve 0b for binary input, even when its digits are invalid.
        if (!input.StartsWith("0b", StringComparison.OrdinalIgnoreCase)
            && input.All(char.IsAsciiHexDigit) && input.Any(CharExtensions.IsHexLetter))
        {
            if (this.TryParseHexadecimal(input, out var hexValue))
            {
                result = new InputFormatParserResult(
                    input,
                    NumberBase.Hexadecimal,
                    hexValue,
                    false,
                    new FormatInfo(FormatStyle.Standard, input.Any(CharExtensions.IsHexLetterUppercase))
                );
                return true;
            }
        }

        // Try to parse as decimal
        if (this.TryParseDecimal(input, out var decValue))
        {
            // Check if it could be binary or octal
            bool couldBeBinary = input.All(CharExtensions.IsBinaryDigit);
            bool couldBeOctal = input.All(CharExtensions.IsOctalDigit);
            bool isImplicit = couldBeBinary || couldBeOctal;

            result = new InputFormatParserResult(
                input,
                NumberBase.Decimal,
                decValue,
                isImplicit,
                new FormatInfo(FormatStyle.Standard)
            );
            return true;
        }

        return false;
    }
}