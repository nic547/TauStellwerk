// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TauStellwerk.Desktop.ViewModels;
using TauStellwerk.Desktop.Views;

namespace TauStellwerk.Desktop;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var context = new MainWindowViewModel();
            DataContext = context;

            desktop.MainWindow = new MainWindow
            {
                DataContext = context,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
