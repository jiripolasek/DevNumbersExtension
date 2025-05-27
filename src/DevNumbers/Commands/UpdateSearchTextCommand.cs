// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.DevNumbers.Helpers;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace JPSoftworks.DevNumbers.Commands;

internal sealed partial class UpdateSearchTextCommand : InvokableCommand
{
    private readonly string _newValue;
    private readonly IDynamicListPage _targetPage;

    public UpdateSearchTextCommand(IDynamicListPage targetPage, string newValue)
    {
        ArgumentNullException.ThrowIfNull(targetPage);
        this._targetPage = targetPage;
        this._newValue = newValue;

        this.Name = newValue;
        this.Icon = Icons.ReturnToCall;
    }

    public override ICommandResult Invoke(object? sender)
    {
        this._targetPage.SearchText = this._newValue;
        return CommandResult.KeepOpen();
    }
}