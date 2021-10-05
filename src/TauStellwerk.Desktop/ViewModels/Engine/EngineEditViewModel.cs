// <copyright file="EngineEditViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels.Engine
{
    public class EngineEditViewModel : ViewModelBase
    {
        private readonly EngineService _engineService;
        private string _tagInputText = string.Empty;

        public EngineEditViewModel(EngineFull engine, EngineService? engineService = null)
        {
            Engine = engine;
            Tags = new ObservableCollection<string>(engine.Tags);

            _engineService = engineService ?? Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();

            SaveCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(HandleSave);
            CancelCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(HandleCancel);
            AddTagCommand = ReactiveCommand.Create<Unit, Unit>(HandleAddTag);
            RemoveTagCommand = ReactiveCommand.Create<string, Unit>(HandleRemoveTag);
        }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReactiveCommand<Unit, Unit> AddTagCommand { get; }

        public ReactiveCommand<string, Unit> RemoveTagCommand { get; }

        public Interaction<Unit, Unit> CloseWindow { get; } = new();

        public EngineFull Engine { get; }

        public ObservableCollection<string> Tags { get; }

        public string TagInputText
        {
            get => _tagInputText;
            set => this.RaiseAndSetIfChanged(ref _tagInputText, value);
        }

        public async void HandleWindowClosing(object? sender, EventArgs e)
        {
            await _engineService.ReleaseEngine(Engine.Id);
        }

        private async Task<Unit> HandleSave(Unit arg)
        {
            await _engineService.AddOrUpdateEngine(Engine);
            await CloseWindow.Handle(arg);
            return Unit.Default;
        }

        private async Task<Unit> HandleCancel(Unit arg)
        {
            await CloseWindow.Handle(arg);
            return Unit.Default;
        }

        private Unit HandleAddTag(Unit arg)
        {
            if (TagInputText != string.Empty)
            {
                Engine.Tags.Add(TagInputText);
                Tags.Add(TagInputText);
                TagInputText = string.Empty;
            }

            return Unit.Default;
        }

        private Unit HandleRemoveTag(string tag)
        {
            Engine.Tags.Remove(tag);
            Tags.Remove(tag);

            return Unit.Default;
        }
    }
}