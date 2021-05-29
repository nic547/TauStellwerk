// <copyright file="Program.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia;
using Avalonia.ReactiveUI;
using PiStellwerk.Client.Services;
using Splat;

namespace PiStellwerk.Desktop
{
    public static class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            Locator.CurrentMutable.RegisterConstant(new SettingsService(), typeof(SettingsService));
            Locator.CurrentMutable.RegisterConstant(new ClientService(), typeof(ClientService));
            Locator.CurrentMutable.RegisterConstant(new EngineService(), typeof(EngineService));
            Locator.CurrentMutable.RegisterConstant(new StatusService(), typeof(StatusService));

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        // ReSharper disable once MemberCanBePrivate.Global
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
