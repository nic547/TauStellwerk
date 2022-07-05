// <copyright file="Program.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia;
using Splat;
using TauStellwerk.Client;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static void Main(string[] args)
    {
        Locator.CurrentMutable.RegisterConstant<ISettingsService>(new SettingsService());
        Locator.CurrentMutable.RegisterConstant(SplatFactory.CreateClientHttpService(), typeof(ConnectionService));
        Locator.CurrentMutable.RegisterConstant(SplatFactory.CreateClientEngineService(), typeof(EngineService));
        Locator.CurrentMutable.RegisterConstant(SplatFactory.CreateClientStatusService(), typeof(StatusService));
        Locator.CurrentMutable.RegisterConstant<IViewService>(new AvaloniaViewService());

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}