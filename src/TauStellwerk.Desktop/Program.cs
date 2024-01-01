// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;
using Splat;
using TauStellwerk.Client.Services;
using TauStellwerk.Client.Services.Connections;
using TauStellwerk.Desktop.Services;
using TauStellwerk.Desktop.Services.WindowSettingService;
using TauStellwerk.Desktop.ViewModels;

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
        AvaloniaViewService avaloniaViewService = new();
        Locator.CurrentMutable.RegisterConstant<IViewService>(avaloniaViewService);
        Locator.CurrentMutable.RegisterConstant<IAvaloniaViewService>(avaloniaViewService);
        Locator.CurrentMutable.RegisterConstant(SplatFactory.CreateTurnoutService());
        Locator.CurrentMutable.RegisterConstant<IWindowSettingService>(new WindowSettingService());

        Locator.CurrentMutable.RegisterConstant(new StopButtonControlViewModel());

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
