// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Globalization;
using System.Numerics;
using System.Text;
using JPSoftworks.DevNumbers.Engine.NumberParsers;
using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Helpers;
using JPSoftworks.DevNumbers.Resources;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Pages;

internal sealed partial class NumericValueListItem : ListItem
{
    private static readonly CompositeFormat CopyCommandTitleFormat = CompositeFormat.Parse(Strings.Command_CopyValue0_Title!);

    public NumericValueListItem(
        BigInteger value,
        NumberBase numberBase,
        string subtitle,
        FormatStyle formatStyle)
    {
        this.Title = NumberParser.FormatWithFallback(value, numberBase, new FormatInfo(formatStyle));
        this.Subtitle = subtitle;
        this.Command = new CopyTextCommand(this.Title) { Name = string.Format(CultureInfo.CurrentCulture, CopyCommandTitleFormat, FormatStyleHelper.FormatStyleToString(formatStyle)) };
        this.Icon = GetIcon(numberBase);

        HashSet<string> tempSet = [];
        this.MoreCommands =
        [
            .. from style in NumberParser.GetSupportedStyles(numberBase)
               where style != formatStyle
               let formattedValue = FormatValue(value, numberBase, style)
               select new
               {
                   FormattedValue = formattedValue,
                   Style = style,
                   CopyName = string.Format(CultureInfo.CurrentCulture, CopyCommandTitleFormat, FormatStyleHelper.FormatStyleToString(style)),
                   CopyValue = string.Format(CultureInfo.CurrentUICulture, CopyCommandTitleFormat, formattedValue)
               }
               into menuItemModel
               where tempSet.Add(menuItemModel.FormattedValue)
               select new CommandContextItem(new CopyTextCommand(menuItemModel.FormattedValue) { Name = menuItemModel.CopyName } ) { Title = menuItemModel.CopyValue }
        ];
    }

    private static IIconInfo? GetIcon(NumberBase numberBase)
    {
        return numberBase switch
        {
            NumberBase.Decimal => Icons.Dial10,
            NumberBase.Hexadecimal => Icons.Dial16,
            NumberBase.Binary => Icons.Dial2,
            NumberBase.Octal => Icons.Dial8,
            NumberBase.Char => Icons.Font,
            _ => (IIconInfo?)null
        };
    }

    private static string FormatValue(
        BigInteger value,
        NumberBase format,
        FormatStyle formatStyle = FormatStyle.Standard)
    {
        return NumberParser.Format(value, format, new FormatInfo(formatStyle));
    }
}