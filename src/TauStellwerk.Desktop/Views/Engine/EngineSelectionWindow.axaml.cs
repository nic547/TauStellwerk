// <copyright file="EngineSelectionWindow.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using TauStellwerk.Desktop.ViewModels.Engine;

namespace TauStellwerk.Desktop.Views.Engine;

public class EngineSelectionWindow : Window
{
    public EngineSelectionWindow(EngineSelectionViewModel vm)
    {
        DataContext = vm;
        vm.ClosingRequested += Close;
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    [UsedImplicitly]
    [Obsolete("Use constructor with ViewModel parameter", true)]
    public EngineSelectionWindow()
    {
        // https://github.com/AvaloniaUI/Avalonia/issues/2593
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}