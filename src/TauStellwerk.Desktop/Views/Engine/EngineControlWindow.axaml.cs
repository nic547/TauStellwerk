// <copyright file="EngineControlWindow.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using TauStellwerk.Desktop.ViewModels.Engine;

namespace TauStellwerk.Desktop.Views.Engine;

public class EngineControlWindow : ReactiveWindow<EngineControlViewModel>
{
    public EngineControlWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Window_OnClosing(object? sender, CancelEventArgs e)
    {
        ViewModel?.OnClosing();
    }
}