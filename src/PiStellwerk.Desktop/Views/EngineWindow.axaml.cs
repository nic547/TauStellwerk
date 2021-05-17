// <copyright file="EngineWindow.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PiStellwerk.Desktop.ViewModels;

namespace PiStellwerk.Desktop.Views
{
    public class EngineWindow : Window
    {
        public EngineWindow()
        {
            DataContext = new EngineViewModel();

            InitializeComponent();
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
