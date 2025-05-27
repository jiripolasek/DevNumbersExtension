// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Runtime.InteropServices;
using Microsoft.CommandPalette.Extensions;

namespace JPSoftworks.DevNumbers;

[Guid("58f5543e-82ff-4a08-be41-a02c84b6f7db")]
public sealed partial class DevNumbers : IExtension, IDisposable
{
    private readonly ManualResetEvent _extensionDisposedEvent;

    private readonly DevNumbersCommandsProvider _provider = new();

    public DevNumbers(ManualResetEvent extensionDisposedEvent)
    {
        this._extensionDisposedEvent = extensionDisposedEvent;
    }

    public object? GetProvider(ProviderType providerType)
    {
        return providerType switch
        {
            ProviderType.Commands => this._provider,
            _ => null
        };
    }

    public void Dispose() => this._extensionDisposedEvent.Set();
}