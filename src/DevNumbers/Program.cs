// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.CommandPalette.Extensions.Toolkit.Helpers;
using JPSoftworks.CommandPalette.Extensions.Toolkit.Logging;
using Microsoft.CommandPalette.Extensions;
using Shmuelie.WinRTServer;
using Shmuelie.WinRTServer.CsWinRT;

namespace JPSoftworks.DevNumbers;

public static class Program
{
    [MTAThread]
    public static async Task Main(string[] args)
    {
        Logger.Initialize("JPSoftworks", "DevNumbers");

        if (args.Length > 0 && args[0] == "-RegisterProcessAsComServer")
        {
            ComServer server = new();

            ManualResetEvent extensionDisposedEvent = new(false);
            
            DevNumbers extensionInstance = new(extensionDisposedEvent);
            server.RegisterClass<DevNumbers, IExtension>(() => extensionInstance);
            server.Start();

            extensionDisposedEvent.WaitOne();
            server.Stop();
            server.UnsafeDispose();
        }
        else
        {
            await StartupHelper.HandleDirectLaunchAsync();
        }
    }
}