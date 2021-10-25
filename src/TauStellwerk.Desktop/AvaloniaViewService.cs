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
using TauStellwerk.Desktop.Views;

namespace TauStellwerk.Desktop
{
    public class AvaloniaViewService : IViewService
    {
        public void ShowEngineControlView(EngineFull engine, object? source = null)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public void ShowEngineSelectionView(object? source = null)
        {
            throw new System.NotImplementedException();
        }

        private static Window? TryGetAssociatedWindow(object? source)
        {
            var appLifetime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

            if (appLifetime == null)
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
            var appLifetime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            return appLifetime?.MainWindow;
        }
    }
}
