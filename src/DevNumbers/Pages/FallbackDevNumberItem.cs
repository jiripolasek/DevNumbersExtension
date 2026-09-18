// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Engine.NumberParsers;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Helpers;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Pages;

internal sealed partial class FallbackDevNumberItem : FallbackCommandItem
{
    private readonly NumberBaseConversionPage _page;
    private readonly SettingsManager _settingsManager;

    public FallbackDevNumberItem(ICommand command, string displayTitle, SettingsManager settingsManager) : base(command, displayTitle, "com.jpsoftworks.cmdpal.devnumbers.conversionFallback")
    {
        this._settingsManager = settingsManager;
        this._page = new NumberBaseConversionPage(settingsManager);
        this.Title = ""; // empty to avoid showing the page title in the fallback item
        this._page.Name = "";
        this.Icon = Icons.BaseConverterPage;
        this.Command = this._page;
    }

    public override void UpdateQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            SetEmpty();
            return;
        }

        try
        {
            var parsedQuery = SwitchParser.Parse(query);
            var result = NumberQuery.Parse(parsedQuery, this._settingsManager.DefaultFormatStyle);
            var formatInfo = new FormatInfo(result.FormatStyle);
            var decimalValue = NumberParser.FormatWithFallback(result.Value, NumberBase.Decimal, formatInfo);
            var hexadecimalValue = NumberParser.FormatWithFallback(result.Value, NumberBase.Hexadecimal, formatInfo);
            var binaryValue = NumberParser.FormatWithFallback(result.Value, NumberBase.Binary, formatInfo);
            var octalValue = NumberParser.FormatWithFallback(result.Value, NumberBase.Octal, formatInfo);

            this._page.Name = "Convert " + query;
            this.Title = decimalValue;
            this._page.SearchText = query;
            this.Subtitle = $"decimal {decimalValue} • hexadecimal {hexadecimalValue} • " +
                $"binary {binaryValue} • octal {octalValue}";
            if (parsedQuery.HasErrors)
            {
                this.Subtitle = $"{parsedQuery.Errors[0].Message} • {this.Subtitle}";
            }

            this.MoreCommands = [
                CommandContextItem(decimalValue, NumberBase.Decimal),
                CommandContextItem(hexadecimalValue, NumberBase.Hexadecimal),
                CommandContextItem(binaryValue, NumberBase.Binary),
                CommandContextItem(octalValue, NumberBase.Octal),
            ];

            return;
        }
        catch (Exception)
        {
            // ignore and skip to noMatch
        }

        SetEmpty();
        return;

        static CommandContextItem CommandContextItem(string formattedValue, NumberBase baseFormat)
        {
            var copyTextCommand = new CopyTextCommand(formattedValue) { Name = $"Copy {baseFormat.ToString().ToLowerInvariant()}" };
            return new CommandContextItem(copyTextCommand) { Title = $"{baseFormat}: {formattedValue}" };
        }

        void SetEmpty()
        {
            this._page.Name = string.Empty;
            this.Title = string.Empty;
            this.Subtitle = string.Empty;
            this.MoreCommands = [];
        }
    }
}