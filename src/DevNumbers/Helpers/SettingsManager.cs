// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Engine.NumberParsers.Abstraction;
using JPSoftworks.DevNumbers.Resources;
using Microsoft.CommandPalette.Extensions.Toolkit;
using static Microsoft.CommandPalette.Extensions.Toolkit.ChoiceSetSetting;

namespace JPSoftworks.DevNumbers.Helpers;

public sealed class SettingsManager : JsonSettingsManager
{
    private const string DefaultNamespace = "jpsoftworks.devnumbers";

    private static readonly List<Choice> DefaultFormatStyleOptions =
    [
        new(Strings.StyleName_Infer!, "-1"),
        new(Strings.StyleName_Standard!, FormatStyle.Standard.ToString("D")),
        new(Strings.StyleName_CSharp!, FormatStyle.CSharpStyle.ToString("D")),
        new(Strings.StyleName_CppStyle!, FormatStyle.CppStyle.ToString("D")),
        new(Strings.StyleName_VBStyle!, FormatStyle.VBStylePrefix.ToString("D")),
        new(Strings.StyleName_AdaStyle!, FormatStyle.AdaStylePrefix.ToString("D")),
        new(Strings.StyleName_RStyle!, FormatStyle.RStylePrefix.ToString("D")),
        new(Strings.StyleName_SingleCharSuffix!, FormatStyle.SingleCharSuffix.ToString("D")),
        new(Strings.StyleName_MultiCharSuffix!, FormatStyle.MultiCharSuffix.ToString("D")),
        new(Strings.StyleName_SingleCharPrefix!, FormatStyle.SingleCharPrefix.ToString("D")),
        new(Strings.StyleName_SpecialCharPrefix!, FormatStyle.SpecialCharPrefix.ToString("D")),
    ];

    private readonly ChoiceSetSetting _defaultFormatStyle = new(
        Namespaced(nameof(DefaultFormatStyle)),
        Strings.Setting_DefaultFormattingStyle_Title!,
        Strings.Setting_DefaultFormattingStyle_Subtitle!,
        DefaultFormatStyleOptions);

    public FormatStyle? DefaultFormatStyle
    {
        get
        {
            var value = this._defaultFormatStyle.Value;
            if (value is null or "-1")
            {
                return null;
            }

            if (Enum.TryParse<FormatStyle>(value, out var result))
            {
                return result;
            }

            return null;
        }
    }

    public SettingsManager()
    {
        this.FilePath = SettingsJsonPath();
        this.Settings.Add(this._defaultFormatStyle);
        this.LoadSettings();
        this.Settings.SettingsChanged += (s, a) => this.SaveSettings();
    }

    private static string Namespaced(string propertyName) => $"{DefaultNamespace}.{propertyName}";


    private static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("Microsoft.CmdPal");
        Directory.CreateDirectory(directory);

        // now, the state is just next to the exe
        return Path.Combine(directory, "settings.json");
    }
}