// <copyright file="AvaloniaViewService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using TauStellwerk.Client;
using TauStellwerk.Client.Model;
using TauStellwerk.Desktop.ViewModels;
using TauStellwerk.Desktop.ViewModels.Engine;
using TauStellwerk.Desktop.Views;
using TauStellwerk.Desktop.Views.Engine;

namespace TauStellwerk.Desktop;

public class AvaloniaViewService : IViewService
{
    public void ShowEngineControlView(EngineFull engine, object? source = null)
    {
        var vm = new EngineControlViewModel(engine);
        var window = new EngineControlWindow(vm);
        ShowWindowCenterOwner(window, TryGetMainWindow(), 0.33);
    }

    public void ShowSettingsView(object? source = null)
    {
        var vm = new SettingsViewModel();
        var window = new SettingsWindow(vm);
        window.Show(TryGetMainWindow());
    }

    public void ShowMessageBox(string title, string message, object? source = null)
    {
        var parentWindow = TryGetAssociatedWindow(source);

        var messageBox = new MessageBox
        {
            DataContext = new MessageBoxModel()
            {
                Title = title,
                Message = message,
            },
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        messageBox.ShowDialog(parentWindow);
    }

    public void ShowEngineEditView(EngineFull engine, object? source = null)
    {
        var vm = new EngineEditViewModel(engine);
        var window = new EngineEditWindow(vm);
        ShowWindowCenterOwner(window, TryGetAssociatedWindow(source), 0.5);
    }

    public void ShowEngineSelectionView(object? source = null)
    {
        var vm = new EngineSelectionViewModel();
        var window = new EngineSelectionWindow(vm);
        window.Show(TryGetMainWindow());
    }

    private static Window? TryGetAssociatedWindow(object? source)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime appLifetime)
        {
            return null;
        }

        foreach (var window in appLifetime.Windows)
        {
            if (window.DataContext == source)
            {
                return window;
            }
        }

        return null;
    }

    private static Window? TryGetMainWindow()
    {
        var appLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return appLifetime?.MainWindow;
    }

    private static void ShowWindowCenterOwner(Window window, Window? parent, double widthMultiplier = 1d)
    {
        if (parent == null)
        {
            window.Show();
            return;
        }

        window.Width = parent.ClientSize.Width * widthMultiplier;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Show(parent);
    }
}