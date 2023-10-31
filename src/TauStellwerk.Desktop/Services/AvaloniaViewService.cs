// <copyright file="AvaloniaViewService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using TauStellwerk.Client.Model;
using TauStellwerk.Desktop.ViewModels;
using TauStellwerk.Desktop.Views;

namespace TauStellwerk.Desktop.Services;

public class AvaloniaViewService : IAvaloniaViewService
{
    public void ShowEngineControlView(EngineFull engine, object? source = null)
    {
        var vm = new EngineControlViewModel(engine);
        var window = new EngineControlWindow(vm);
        window.Show();
    }

    public void ShowSettingsView(object? source = null)
    {
        var vm = new SettingsViewModel();
        var window = new SettingsWindow(vm);
        window.Show();
    }

    public void ShowMessageBox(string title, string message, object source)
    {
        var parentWindow = TryGetAssociatedWindow(source) ?? throw new InvalidOperationException("Failed to locate source of dialog request");

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

    public void ShowEngineEditView(EngineFull engine, object? source)
    {
        var vm = new EngineEditViewModel(engine);
        var window = new EngineEditWindow(vm);
        var sourceWindow = TryGetAssociatedWindow(source) ?? throw new InvalidOperationException("Failed to determine the source window opening a new EngineEditView");

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
        window.Show();
    }

    public void ShowEngineSelectionView(object? source = null)
    {
        var vm = new EngineSelectionViewModel();
        var window = new EngineSelectionWindow(vm);
        window.Show();
    }

    public void ShowTurnoutsWindow(object? source = null)
    {
        var vm = new TurnoutsViewModel();
        var window = new TurnoutsWindow(vm);
        window.Show();
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
            window.Show();
        }
    }

    public async Task<IStorageFile?> ShowFilePicker(object source, FilePickerOpenOptions? filePickerOpenOptions = null)
    {
        var window = TryGetAssociatedWindow(source) ?? throw new InvalidOperationException("Failed to locate window associated with viewmodel.");

        var file = await window.StorageProvider.OpenFilePickerAsync(
            filePickerOpenOptions ?? new FilePickerOpenOptions());

        return file.SingleOrDefault();
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
}