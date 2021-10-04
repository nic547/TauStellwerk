﻿// <copyright file="EngineSelectionViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using ReactiveUI;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Services;
using TauStellwerk.Util;

namespace TauStellwerk.Desktop.ViewModels.Engine
{
    public class EngineSelectionViewModel : ViewModelBase
    {
        private readonly EngineService _engineService;
        private readonly ObservableAsPropertyHelper<bool> _isAcquiring;

        private Size _windowSize;

        private bool _showHiddenEngines;
        private SortEnginesBy _currentEngineSortMode = SortEnginesBy.LastUsed;
        private string _currentEngineSortDirection = "DESC";

        private int _currentPage;

        public EngineSelectionViewModel(EngineService? engineService = null)
        {
            _engineService = engineService ?? Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();
            _ = Load((CurrentPage, CurrentEngineSortMode, CurrentEngineSortDirection, ShowHiddenEngines));

            AcquireEngine = ReactiveCommand.CreateFromTask<int, Unit>(TryAcquireEngine);
            AcquireEngine.IsExecuting.ToProperty(this, x => x.IsAcquiring, out _isAcquiring);

            EditEngineCommand = ReactiveCommand.CreateFromTask<int, Unit>(HandleEngineEditCommand);

            this.WhenAnyValue(
                    v => v.CurrentPage,
                    v => v.CurrentEngineSortMode,
                    v => v.CurrentEngineSortDirection,
                    v => v.ShowHiddenEngines)
                .Select(values => _ = Load(values))
                .Subscribe();
        }

        public static SortEnginesBy[] EngineSortModes => Enum.GetValues<SortEnginesBy>();

        public static string[] EngineSortDirections => new[] { "ASC", "DESC" };

        public Size WindowSize
        {
            get => _windowSize;
            set
            {
                this.RaiseAndSetIfChanged(ref _windowSize, value);
                this.RaisePropertyChanged(nameof(Columns));
            }
        }

        public int Columns
        {
            get
            {
                return WindowSize.Width switch
                {
                    < 768 => 1,
                    < 992 => 2,
                    < 1400 => 4,
                    _ => 5,
                };
            }
        }

        public Interaction<EngineFullDto, Unit> SelectEngine { get; } = new();

        public Interaction<Unit, Unit> CannotAcquireEngineError { get; } = new();

        public Interaction<EngineFullDto, Unit> OpenEngineEditView { get; } = new();

        public ReactiveCommand<int, Unit> AcquireEngine { get; }

        public ReactiveCommand<int, Unit> EditEngineCommand { get; }

        public ObservableCollection<EngineDto> Engines { get; } = new();

        public int CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value.Clamp());
        }

        public bool CanScrollForwards => Engines.Any();

        public bool CanScrollBackwards => CurrentPage > 0;

        public SortEnginesBy CurrentEngineSortMode
        {
            get => _currentEngineSortMode;
            set => this.RaiseAndSetIfChanged(ref _currentEngineSortMode, value);
        }

        public string CurrentEngineSortDirection
        {
            get => _currentEngineSortDirection;
            set => this.RaiseAndSetIfChanged(ref _currentEngineSortDirection, value);
        }

        public bool ShowHiddenEngines
        {
            get => _showHiddenEngines;
            set => this.RaiseAndSetIfChanged(ref _showHiddenEngines, value);
        }

        public bool IsAcquiring => _isAcquiring.Value;

        public void ScrollPages(int change)
        {
            CurrentPage += change;
        }

        private async Task<Unit> TryAcquireEngine(int id)
        {
            var engine = await _engineService.AcquireEngine(id);
            if (engine != null)
            {
                await SelectEngine.Handle(engine);
            }
            else
            {
                await CannotAcquireEngineError.Handle(Unit.Default);
            }

            return Unit.Default;
        }

        private async Task Load((int Page, SortEnginesBy SortBy, string SortDirection, bool ShowHidden) args)
        {
            var sortDescending = args.SortDirection == "DESC";
            var engines = await _engineService.GetEngines(args.Page, args.SortBy, sortDescending);

            Engines.Clear();
            foreach (var engine in engines)
            {
                Engines.Add(engine);
            }

            this.RaisePropertyChanged(nameof(CanScrollForwards));
            this.RaisePropertyChanged(nameof(CanScrollBackwards));
        }

        private async Task<Unit> HandleEngineEditCommand(int id)
        {
            var engine = await _engineService.AcquireEngine(id);
            if (engine != null)
            {
                await OpenEngineEditView.Handle(engine);
            }
            else
            {
                await CannotAcquireEngineError.Handle(Unit.Default);
            }

            return Unit.Default;
        }
    }
}