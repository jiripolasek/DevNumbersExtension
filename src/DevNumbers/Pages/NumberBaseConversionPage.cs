// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.CommandPalette.Extensions.Toolkit.Logging;
using JPSoftworks.DevNumbers.Engine;
using JPSoftworks.DevNumbers.Engine.NumberParsers;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Helpers;
using JPSoftworks.DevNumbers.Resources;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Pages;

internal sealed partial class NumberBaseConversionPage : DynamicListPage
{
    private readonly SettingsManager _settingsManager;

    private IListItem[] _results = [];

    public NumberBaseConversionPage(SettingsManager settingsManager)
    {
        this._settingsManager = settingsManager;
        this.Icon = Icons.BaseConverterPage;
        this.Title = Strings.NumberBaseConversionPage_Title!;
        this.Name = Strings.NumberBaseConversionPage_Title!;
        this.PlaceholderText = Strings.NumberBaseConversionPage_Placeholder!;
        this.Id = "com.jpsoftworks.cmdpal.devnumbers.conversions";

        this.EmptyContent = BuildEmptyContent();
    }

    public override IListItem[] GetItems()
    {
        return this._results;
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        if (oldSearch == newSearch)
        {
            return;
        }

        var failed = false;
        InputFormatParserResult? numberParseResult = null;
        ParseResult? queryParseResult = null;
        try
        {
            // parse out switches and options out of newSearch
            queryParseResult = SwitchParser.Parse(newSearch);

            numberParseResult = NumberParser.Parse(queryParseResult.Query);

            if (numberParseResult == null || numberParseResult.NumberBase == NumberBase.Unknown)
            {
                failed = true;
            }
            else
            {
                var explicitBitLength = queryParseResult.Options.BitLength;
                var actualBitLength = queryParseResult.Options.BitLength;
                var actualValue = numberParseResult.Value;

                // a) if bit length is specified, then we apply it to the parsed input:
                // - if input bit-size is bigger that bit length, then be have to fail
                // b) if not specified
                // b.1) if input is negative, then let's automatically determine the best bit length
                // b.2) if input is positive, let's don't care about bit length
                if (explicitBitLength > -1)
                {
                    actualBitLength = explicitBitLength;
                    actualValue = numberParseResult.Value.TrimToBitLength(actualBitLength);
                }
                else
                {
                    if (actualValue < 0)
                    {
                        actualBitLength = actualValue.GetNearestContainerBitLength(true);
                    }
                    // if input is positive, then we don't care about bit length
                }

                // What to show to the user:
                // 1) Value as returned by parser
                //   - make this special case, because we want to show the original input and attach additional information (like real bit length) and extra commands to it
                // 2) Check for alternative meanings (e.g. if decimal contains only digits 1 or 0 then it might be binary)
                // 3) Signed decimal
                // 4) If decimal is negative, then add unsigned decimal
                // 5) Hexadecimal
                // 6) Binary
                // 7) Octal
                // 8) Char (if applicable)

                var targetStyle = this._settingsManager.DefaultFormatStyle ?? numberParseResult.FormatInfo.Style;

                List<IListItem> results =
                [
                    new ParsedValueListItem(numberParseResult, explicitBitLength),
                    ..this.AddDidYouMean(numberParseResult),
                    new NumericValueListItem(actualValue, NumberBase.Decimal, Strings.NumberBase_DecimalLabel!, targetStyle)
                ];

                if (queryParseResult.HasErrors)
                {
                    results.Insert(0,
                        new ListItem
                        {
                            Icon = Icons.Warning,
                            Title = Strings.ErrorInQuery!,
                            Subtitle = queryParseResult.Errors[0].Message
                        });
                }


                // If the number is not decimal, let's assume we have no idea about its actual sign
                // Display signed and unsigned decimal values if applicable
                if (numberParseResult.NumberBase != NumberBase.Char)
                {
                    if (actualValue > 0)
                    {
                        var signedValue = actualValue.InterpretAsSigned(actualBitLength > -1
                            ? actualBitLength
                            : actualValue.GetNearestContainerBitLength(failed));
                        if (signedValue != actualValue)
                        {
                            results.Add(new NumericValueListItem(signedValue, NumberBase.Decimal, Strings.NumberBase_DecimalSigned!, targetStyle));
                        }
                    }

                    if (actualValue < 0)
                    {
                        var unsignedValue = actualValue.InterpretAsUnsigned(actualBitLength > -1
                            ? actualBitLength
                            : actualValue.GetNearestContainerBitLength(failed));
                        results.Add(new NumericValueListItem(unsignedValue, NumberBase.Decimal, Strings.NumberBase_DecimalUnsigned!, targetStyle));
                    }
                }

                // Other number bases
                results.AddRange([
                    new NumericValueListItem(actualValue, NumberBase.Hexadecimal, Strings.NumberBase_Hexadecimal!, targetStyle),
                    new NumericValueListItem(actualValue, NumberBase.Binary, Strings.NumberBase_Binary!, targetStyle),
                    new NumericValueListItem(actualValue, NumberBase.Octal, Strings.NumberBase_Octal!, targetStyle)
                ]);

                // Byte swapping is meaningful only when the value produced after any
                // explicit /length truncation fits a two-byte signed or unsigned pattern.
                // The standard numeric list item keeps its title, copy command, alternate
                // format commands, and base icon consistent with the existing result rows.
                if (ByteOrderHelper.TryByteSwap16(actualValue, out var byteSwappedValue))
                {
                    // Preserve the input base so the new row reads naturally beside the
                    // parsed value: decimal 2 becomes 512, while 0x1234 becomes 0x3412.
                    // Character literals cannot safely display an arbitrary swapped bit
                    // pattern as text, so hexadecimal is the unambiguous fallback for them.
                    var byteSwappedNumberBase = numberParseResult.NumberBase == NumberBase.Char
                        ? NumberBase.Hexadecimal
                        : numberParseResult.NumberBase;

                    results.Add(new NumericValueListItem(
                        byteSwappedValue,
                        byteSwappedNumberBase,
                        Strings.NumberBase_ByteSwapped16Bit!,
                        targetStyle));
                }

                // Let's try to interpret the value as a character or string if applicable
                // Not really primary goal of this page, but still useful 
                try
                {
                   var characterInterpretations = BigIntegerCharacterInference.GetValidCharacterInterpretations(actualValue);
                   foreach (var characterResult in characterInterpretations)
                   {
                       results.Add(new ListItem
                       {
                           Title = characterResult.Character,
                           Subtitle = $"{(characterResult.Character.Length > 1 ? "string" : "character")} • {characterResult.Encoding} • {characterResult.HexRepresentation}",
                           Icon = Icons.Characters,
                           Command = new CopyTextCommand(characterResult.Character)
                       });
                   }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }


                this._results = [.. results];
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            failed = true;
        }

        if (failed)
        {
            if (string.IsNullOrWhiteSpace(newSearch) || (queryParseResult != null && string.IsNullOrWhiteSpace(queryParseResult.Query)))
            {
                this.EmptyContent = BuildEmptyContent();
            }
            else if (numberParseResult == null || numberParseResult.NumberBase == NumberBase.Unknown)
            {
                this.EmptyContent = BuildEmptyContent2();
            }

            this._results = [];
        }

        this.RaiseItemsChanged(this._results.Length);
    }

    private static CommandItem BuildEmptyContent2()
    {
        return new CommandItem
        {
            Title = "Unrecognized input",
            Subtitle = "You can use decimal, hexadecimal, binary, octal or char formats. " +
                       "You can also use commands like '123' or '0xFF' to parse numbers.",
            Icon = Icons.StoreLogo
        };
    }

    private static CommandItem BuildEmptyContent()
    {
        return new CommandItem
        {
            Title = "Type a number to parse",
            Subtitle = "You can use decimal, hexadecimal, binary, octal or char formats. " +
                       "You can also use commands like '123' or '0xFF' to parse numbers.",
            Icon = Icons.StoreLogo
        };
    }

    private IEnumerable<IListItem> AddDidYouMean(InputFormatParserResult parsedInput)
    {
        return parsedInput.NumberBase == NumberBase.Decimal && parsedInput.RawValue.All(static c => c is '1' or '0')
            ? [new DidYouMeanBinaryListItem(parsedInput.RawValue, parsedInput.NumberBase, this)]
            : [];
    }
}