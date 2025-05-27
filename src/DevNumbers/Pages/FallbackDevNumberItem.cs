// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;
using System.Text;
using JPSoftworks.DevNumbers.Engine.NumberParsers;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Helpers;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Pages;

internal sealed partial class FallbackDevNumberItem : FallbackCommandItem
{
    private readonly NumberBaseConversionPage _page;

    public FallbackDevNumberItem(ICommand command, string displayTitle, SettingsManager settingsManager) : base(command, displayTitle)
    {
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
            var result = NumberParser.Parse(query);
            if (result != null && result.NumberBase != NumberBase.Unknown)
            {
                this._page.Name = "Convert " + query;
                this.Title = result.Value.ToString(CultureInfo.InvariantCulture);
                this._page.SearchText = result.RawValue;

                var sb = new StringBuilder(64);
                sb.Append("decimal ").Append(NumberParser.Convert(result, NumberBase.Decimal).RawValue).Append(" • ");
                sb.Append("hexadecimal ").Append(NumberParser.Convert(result, NumberBase.Hexadecimal).RawValue).Append(" • ");
                sb.Append("binary ").Append(NumberParser.Convert(result, NumberBase.Binary).RawValue).Append(" • ");
                sb.Append("octal ").Append(NumberParser.Convert(result, NumberBase.Octal).RawValue);
                this.Subtitle = sb.ToString();

                this.MoreCommands = [
                    CommandContextItem(result, NumberBase.Decimal),
                    CommandContextItem(result, NumberBase.Hexadecimal),
                    CommandContextItem(result, NumberBase.Binary),
                    CommandContextItem(result, NumberBase.Octal),
                ];

                return;
            }
        }
        catch (Exception)
        {
            // ignore and skip to noMatch
        }

        SetEmpty();
        return;

        static CommandContextItem CommandContextItem(InputFormatParserResult result, NumberBase baseFormat)
        {
            var formattedValue = NumberParser.Convert(result, baseFormat).RawValue;
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