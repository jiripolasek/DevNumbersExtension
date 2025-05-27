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
        this.Command = new CopyTextCommand(this.Title);
        this.Icon = GetIcon(numberBase);
        
        HashSet<string> tempSet = [];
        this.MoreCommands =
        [
            .. from item in NumberParser.GetSupportedStyles(numberBase)
               select FormatValue(value, numberBase, item)
               into formatValue
               where tempSet.Add(formatValue)
               select new CommandContextItem(new CopyTextCommand(formatValue)) { Title = string.Format(CultureInfo.CurrentUICulture, CopyCommandTitleFormat, formatValue) }
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