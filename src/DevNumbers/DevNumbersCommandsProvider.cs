// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Helpers;
using JPSoftworks.DevNumbers.Pages;
using JPSoftworks.DevNumbers.Resources;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers;

public sealed partial class DevNumbersCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;
    private readonly IFallbackCommandItem[] _fallbackCommands;
    private readonly SettingsManager _settingsManager = new();

    public override ICommandItem[] TopLevelCommands() => this._commands;

    public override IFallbackCommandItem[] FallbackCommands() => this._fallbackCommands;

    public DevNumbersCommandsProvider()
    {
        this.DisplayName = Strings.ProgrammerNumbers_Title!;
        this.Icon = Icons.BaseConverterPage;
        this.Settings = this._settingsManager.Settings;

        this._fallbackCommands =
        [
            new FallbackDevNumberItem(new NumberBaseConversionPage(this._settingsManager), Strings.ProgrammerNumbers_Title!, this._settingsManager)
        ];

        this._commands =
        [
            new CommandItem(new NumberBaseConversionPage(this._settingsManager)) {
                Title = Strings.NumberBaseConversionPage_Title!,
                Subtitle = Strings.NumberBaseConversionPage_Subtitle!,
                Icon = Icons.BaseConverterPage,
                MoreCommands = [
                    new CommandContextItem(this.Settings!.SettingsPage!),
                ]}
        ];
    }
}