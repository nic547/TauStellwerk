// <copyright file="AvaloniaViewService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using FluentResults;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;
using TauStellwerk.Desktop.ViewModels;
using TauStellwerk.Desktop.Views;

namespace TauStellwerk.Desktop;

public class AvaloniaViewService : IViewService
{
    public void ShowEngineControlView(EngineFull engine, object? source = null)
    {
        var vm = new EngineControlViewModel(engine);
        var window = new EngineControlWindow(vm);
        ShowWindowCenterOwner(window, GetMainWindow(), 0.33);
    }

    public void ShowSettingsView(object? source = null)
    {
        var vm = new SettingsViewModel();
        var window = new SettingsWindow(vm);
        window.Show(GetMainWindow());
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

        messageBox.ShowDialog(parentWindow ?? GetMainWindow());
    }

    public void ShowEngineEditView(EngineFull engine, object? source)
    {
        var vm = new EngineEditViewModel(engine);
        var window = new EngineEditWindow(vm);
        var sourceWindow = TryGetAssociatedWindow(source);

        if (sourceWindow is null)
        {
            throw new InvalidOperationException("Failed to determine the source window opening a new EngineEditView");
        }

        if (sourceWindow is EngineEditWindow)
        {
            window.Width = sourceWindow.Width;
        }
        else
        {
            window.Width = sourceWindow.Width * 0.5;
        }

        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.Position = new PixelPoint(sourceWindow.Position.X + 50, sourceWindow.Position.Y + 50);
        window.Show(GetMainWindow());
    }

    public void ShowEngineSelectionView(object? source = null)
    {
        var vm = new EngineSelectionViewModel();
        var window = new EngineSelectionWindow(vm);
        window.Show(GetMainWindow());
    }

    public void ShowTurnoutsWindow(object? source = null)
    {
        var vm = new TurnoutsViewModel();
        var window = new TurnoutsWindow(vm);
        window.Show(GetMainWindow());
    }

    public void ShowTurnoutEditWindow(Turnout turnout, object? source = null)
    {
        var vm = new TurnoutEditViewModel(turnout);
        var window = new TurnoutEditWindow(vm);
        var associatedWindow = TryGetAssociatedWindow(source);
        if (associatedWindow is null)
        {
            window.Show();
        }
        else
        {
            window.Show(associatedWindow);
        }
    }

    public async Task<IStorageFile> ShowFilePicker(object source)
    {
        var window = TryGetAssociatedWindow(source) ?? throw new InvalidOperationException("Failed to locate window associated with viewmodel.");

        var file = await window.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions());

        return file.Single();
    }

    /// <summary>
    /// Tries to find the window of a given DataContext.
    /// </summary>
    /// <param name="source">The DataContext.</param>
    /// <returns>The window.</returns>
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

    private static Window GetMainWindow()
    {
        var appLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return appLifetime?.MainWindow ?? throw new Exception("Failed to find ApplicationLifetime.");
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