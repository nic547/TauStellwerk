// <copyright file="EngineSelectionWindow.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using TauStellwerk.Base.Model;
using TauStellwerk.Desktop.ViewModels;
using TauStellwerk.Desktop.ViewModels.Engine;

namespace TauStellwerk.Desktop.Views.Engine
{
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
                ViewModel?.CannotAcquireEngineError.RegisterHandler(ShowAcquireEngineMessage);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OpenEngineWindow(InteractionContext<EngineFullDto, Unit> interaction)
        {
            var vm = new EngineControlViewModel(interaction.Input);
            var view = new EngineControlWindow()
            {
                DataContext = vm,
            };

            view.Show();
            Close();
        }

        private void ShowAcquireEngineMessage(InteractionContext<Unit, Unit> interaction)
        {
            var window = new MessageBox
            {
                ViewModel = new MessageBoxModel
                {
                    Title = "Cannot acquire engine",
                    Message = "Engine seems to be in use already.",
                },
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            window.ShowDialog(this);
        }
    }
}
