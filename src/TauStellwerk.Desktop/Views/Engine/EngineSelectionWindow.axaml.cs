// <copyright file="EngineSelectionWindow.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Reactive;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TauStellwerk.Client.Model;
using TauStellwerk.Desktop.ViewModels.Engine;

namespace TauStellwerk.Desktop.Views.Engine;

public class EngineSelectionWindow : ReactiveWindow<EngineSelectionViewModel>
{
    public EngineSelectionWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.WhenActivated(d =>
        {
            ViewModel?.SelectEngine.RegisterHandler(OpenEngineWindow);
            ViewModel?.OpenEngineEditView.RegisterHandler(OpenEngineEditWindow);
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OpenEngineWindow(InteractionContext<EngineFull, Unit> interaction)
    {
        var vm = new EngineControlViewModel(interaction.Input);
        var view = new EngineControlWindow()
        {
            DataContext = vm,
        };

        view.Show();
        Close();
        interaction.SetOutput(Unit.Default);
    }

    private void OpenEngineEditWindow(InteractionContext<EngineFull, Unit> interaction)
    {
        var vm = new EngineEditViewModel(interaction.Input);
        var view = new EngineEditWindow()
        {
            DataContext = vm,
        };

        view.Show();
        interaction.SetOutput(Unit.Default);
    }
}