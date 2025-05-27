// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;
using System.Text;
using JPSoftworks.DevNumbers.Commands;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Resources;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Pages;

internal sealed partial class DidYouMeanBinaryListItem : ListItem
{
    private static readonly CompositeFormat TitleComposable = CompositeFormat.Parse(Strings.QueryItem_DidYouMean0AsBinary!);

    public DidYouMeanBinaryListItem(string rawInput, NumberBase format, DynamicListPage parent)
    {
        this.Title = string.Format(CultureInfo.InvariantCulture, TitleComposable, format);
        this.TextToSuggest = rawInput + "bin";
        this.Subtitle = format.ToString().ToLowerInvariant();
        this.Command = new UpdateSearchTextCommand(parent, this.TextToSuggest);
    }
}