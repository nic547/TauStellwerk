// <copyright file="EngineEditWindow.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Reactive;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TauStellwerk.Desktop.ViewModels.Engine;

namespace TauStellwerk.Desktop.Views.Engine;

public class EngineEditWindow : ReactiveWindow<EngineEditViewModel>
{
    public EngineEditWindow()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel == null)
            {
                throw new InvalidOperationException();
            }

            ViewModel.CloseWindow.RegisterHandler(HandleCloseWindow);
            Closing += ViewModel.HandleWindowClosing;
        });

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void HandleCloseWindow(InteractionContext<Unit, Unit> context)
    {
        Close();
        context.SetOutput(Unit.Default);
    }
}