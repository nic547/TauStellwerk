// <copyright file="EngineEditViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels.Engine
{
    public class EngineEditViewModel : ViewModelBase
    {
        private readonly EngineService _engineService;

        public EngineEditViewModel(EngineFullDto engine, EngineService? engineService = null)
        {
            Engine = engine;

            _engineService = engineService ?? Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();

            SaveCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(HandleSave);
            CancelCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(HandleCancel);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public Interaction<Unit, Unit> CloseWindow { get; } = new();

        public EngineFullDto Engine { get; }

        private async Task<Unit> HandleSave(Unit arg)
        {
            await _engineService.AddOrUpdateEngine(Engine);
            return await HandleCancel(arg);
        }

        private async Task<Unit> HandleCancel(Unit arg)
        {
            await _engineService.ReleaseEngine(Engine.Id);
            await CloseWindow.Handle(arg);
            return Unit.Default;
        }
    }
}