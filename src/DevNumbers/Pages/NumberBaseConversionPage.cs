// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Numerics;
using JPSoftworks.CommandPalette.Extensions.Toolkit.Logging;
using JPSoftworks.DevNumbers.Engine;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Helpers;
using JPSoftworks.DevNumbers.Resources;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Pages;

internal sealed partial class NumberBaseConversionPage : DynamicListPage
{
    private readonly SettingsManager _settingsManager;

    private IListItem[] _results;
    private readonly ListItem _typeNumberListItem;
    private readonly ListItem _unrecognizedNumberListItem;

    public NumberBaseConversionPage(SettingsManager settingsManager)
    {
        this._settingsManager = settingsManager;
        this.Icon = Icons.BaseConverterPage;
        this.Title = Strings.NumberBaseConversionPage_Title!;
        this.Name = Strings.NumberBaseConversionPage_Title!;
        this.PlaceholderText = Strings.NumberBaseConversionPage_Placeholder!;
        this.Id = "com.jpsoftworks.cmdpal.devnumbers.conversions";

        this._typeNumberListItem = new ListItem(new HelpPage())
        {
            Icon = Icons.Info,
            Title = "Type a number",
            Subtitle = "Try: 123, 0xFF33, 0b1010, #1A2B3C, 1FFh, or 1010bin. For details on all formats and options, open help.",
            MoreCommands = [
                new CommandContextItem(this._settingsManager.Settings.SettingsPage),
            ]
        };
        this._unrecognizedNumberListItem = new ListItem(new HelpPage())
        {
            Icon = Icons.Warning,
            Title = "Number not recognized",
            Subtitle = "Try: 123, 0xFF33, 0b1010, #1A2B3C, 1FFh, or 1010bin. For details on all formats and options, open help.",
            MoreCommands = [
                new CommandContextItem(this._settingsManager.Settings.SettingsPage),
            ]
        };
        this._results = [this._typeNumberListItem];
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

        try
        {
            this.UpdateSearchTextCore(newSearch);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    private void UpdateSearchTextCore(string newSearch)
    {
        try
        {
            var queryParseResult = SwitchParser.Parse(newSearch);

            List<IListItem> results = [];
            if (queryParseResult.HasErrors)
            {
                foreach (var parseError in queryParseResult.Errors)
                {
                    results.Add(new ListItem
                    {
                        Icon = Icons.Warning,
                        Title = Strings.ErrorInQuery!,
                        Subtitle = parseError.Message
                    });
                }
            }

            if (string.IsNullOrWhiteSpace(queryParseResult.Query))
            {
                results.Add(this._typeNumberListItem);
            }
            else
            {
                this.ConvertNumber(queryParseResult, ref results);
            }

            this._results = [.. results];
            this.RaiseItemsChanged(this._results.Length);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    private void ConvertNumber(ParseResult queryParseResult, ref List<IListItem> results)
    {
        NumberQuery numberQuery;
        try
        {
            numberQuery = NumberQuery.Parse(queryParseResult, this._settingsManager.DefaultFormatStyle);
        }
        catch (FormatException)
        {
            results.Add(this._unrecognizedNumberListItem);
            return;
        }

        this.BuildConversionListItems(queryParseResult.Options, numberQuery, results);
    }

    private void BuildConversionListItems(Options options, NumberQuery numberQuery, List<IListItem> results)
    {
        var numberParseResult = numberQuery.Number;
        var explicitBitLength = options.BitLength;
        var actualBitLength = numberQuery.BitLength;
        var actualValue = numberQuery.Value;

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

        var targetStyle = numberQuery.FormatStyle;

        results.Add(new ParsedValueListItem(numberParseResult, explicitBitLength));
        results.AddRange(this.AddDidYouMean(numberParseResult));
        results.Add(new NumericValueListItem(actualValue, NumberBase.Decimal, Strings.NumberBase_DecimalLabel!, targetStyle));

        AddExtraDecimalEntry(numberParseResult, actualValue, actualBitLength, results, targetStyle);

        AddPower2NumberBases(results, actualValue, targetStyle);

        // Swap the value after applying /length truncation.
        if (ByteOrderHelper.TryByteSwap16(actualValue, out var byteSwappedValue))
        {
            var byteSwappedNumberBase = numberParseResult.NumberBase == NumberBase.Char
                ? NumberBase.Hexadecimal
                : numberParseResult.NumberBase;

            results.Add(new NumericValueListItem(
                byteSwappedValue,
                byteSwappedNumberBase,
                Strings.NumberBase_ByteSwapped16Bit!,
                targetStyle));
        }

        TryToAddCharacterInterpretations(actualValue, results);
    }

    private static void AddExtraDecimalEntry(InputFormatParserResult numberParseResult, BigInteger actualValue,
        int actualBitLength, List<IListItem> results, FormatStyle targetStyle)
    {
        // If the number is not decimal, let's assume we have no idea about its actual sign
        // Display signed and unsigned decimal values if applicable
        if (numberParseResult.NumberBase != NumberBase.Char)
        {
            if (actualValue > 0)
            {
                var signedValue = actualValue.InterpretAsSigned(actualBitLength > -1
                    ? actualBitLength
                    : actualValue.GetNearestContainerBitLength());
                if (signedValue != actualValue)
                {
                    results.Add(new NumericValueListItem(signedValue, NumberBase.Decimal, Strings.NumberBase_DecimalSigned!, targetStyle));
                }
            }

            if (actualValue < 0)
            {
                var unsignedValue = actualValue.InterpretAsUnsigned(actualBitLength > -1
                    ? actualBitLength
                    : actualValue.GetNearestContainerBitLength());
                results.Add(new NumericValueListItem(unsignedValue, NumberBase.Decimal, Strings.NumberBase_DecimalUnsigned!, targetStyle));
            }
        }
    }

    private static void AddPower2NumberBases(List<IListItem> results, BigInteger actualValue, FormatStyle targetStyle)
    {
        results.Add(new NumericValueListItem(actualValue, NumberBase.Hexadecimal, Strings.NumberBase_Hexadecimal!, targetStyle));
        results.Add(new NumericValueListItem(actualValue, NumberBase.Binary, Strings.NumberBase_Binary!, targetStyle));
        results.Add(new NumericValueListItem(actualValue, NumberBase.Octal, Strings.NumberBase_Octal!, targetStyle));
    }

    private static void TryToAddCharacterInterpretations(BigInteger actualValue, List<IListItem> results)
    {
        // Let's try to interpret the value as a character or string if applicable
        // Not really primary goal of this page, but still useful

        try
        {
            var characterInterpretations = BigIntegerCharacterInference.GetValidCharacterInterpretations(actualValue);
            foreach (var characterResult in characterInterpretations)
            {
                results.Add(new ListItem
                {
                    Title = characterResult.DisplayCharacter,
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
    }

    private IListItem[] AddDidYouMean(InputFormatParserResult parsedInput)
    {
        return parsedInput.NumberBase == NumberBase.Decimal && parsedInput.RawValue.All(static c => c is '1' or '0')
            ? [new DidYouMeanBinaryListItem(parsedInput.RawValue, parsedInput.NumberBase, this)]
            : [];
    }
}