// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

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
    private readonly object settingsWindowLock = new();

    public void ShowEngineControlView(EngineFull engine, object? source = null)
    {
        var vm = new EngineControlViewModel(engine);
        var window = new EngineControlWindow(vm);
        window.Show();
    }

    public void ShowSettingsView(object? source = null)
    {
        // Opening multiple setting windows will cause them to intefere with each other.
        // So we only show one settings window at a time.

        lock (settingsWindowLock)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime appLifetime)
            {
                foreach (var existingWindow in appLifetime.Windows)
                {
                    if (existingWindow.GetType() == typeof(SettingsWindow))
                    {
                        if (existingWindow.WindowState == WindowState.Minimized)
                        {
                            existingWindow.WindowState = WindowState.Normal;
                        }

                        existingWindow.Activate();
                        return;
                    }
                }
            }

            var vm = new SettingsViewModel();
            var window = new SettingsWindow(vm);
            window.Show();

        }
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

    public void ShowProgrammingWindow()
    {
        var vm = new DecoderProgrammingViewModel();
        var window = new DecoderProgrammingWindow(vm);
        window.Show();
    }

    public void ShowDataTransferWindow()
    {
        new DataTransferWindow().Show();
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
