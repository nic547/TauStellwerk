// <copyright file="SettingsWindow.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Views
{
    public class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
