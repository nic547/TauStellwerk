// <copyright file="EngineViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Primitives;
using JetBrains.Annotations;
using ReactiveUI;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Services;
using TauStellwerk.Util;

namespace TauStellwerk.Desktop.ViewModels
{
    public class EngineViewModel : ViewModelBase
    {
        private readonly ClientEngineService _engineService;
        private readonly ObservableCollection<EngineDto> _engines = new();

        private Size _windowSize;

        private bool _showHiddenEngines;
        private SortEnginesBy _currentEngineSortMode = SortEnginesBy.LastUsed;
        private string _currentEngineSortDirection = "DESC";

        private EngineFullDto? _activeEngine;
        private bool _isInSelectionMode = true;
        private bool _isDrivingForward = true;
        private int _throttle;
        private int _currentPage;

        public EngineViewModel(ClientEngineService? engineService = null)
        {
            _engineService = engineService ?? Locator.Current.GetService<ClientEngineService>() ?? throw new InvalidOperationException();
            Load((CurrentPage, CurrentEngineSortMode, CurrentEngineSortDirection, ShowHiddenEngines));

            this.WhenAnyValue(v => v.Throttle).Throttle(TimeSpan.FromMilliseconds(50)).Subscribe(HandleThrottleChange);
            this.WhenAnyValue(
                    v => v.CurrentPage,
                    v => v.CurrentEngineSortMode,
                    v => v.CurrentEngineSortDirection,
                    v => v.ShowHiddenEngines)
                .Subscribe(Load);
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

        public ObservableCollection<EngineDto> Engines => _engines;

        public EngineFullDto? ActiveEngine
        {
            get => _activeEngine;
            set
            {
                this.RaiseAndSetIfChanged(ref _activeEngine, value);
                this.RaisePropertyChanged(nameof(SortedFunctions));
            }
        }

        public List<FunctionDto>? SortedFunctions => _activeEngine?.Functions.OrderBy(f => f.Number).ToList();

        public bool IsInSelectionMode
        {
            get => _isInSelectionMode;
            set => this.RaiseAndSetIfChanged(ref _isInSelectionMode, value);
        }

        public int Throttle
        {
            get => _throttle;
            set => this.RaiseAndSetIfChanged(ref _throttle, value);
        }

        public bool IsDrivingForward
        {
            get => _isDrivingForward;
            set => this.RaiseAndSetIfChanged(ref _isDrivingForward, value);
        }

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

        public async Task SelectEngine(int id)
        {
            var engine = await _engineService.AcquireEngine(id);
            if (engine != null)
            {
                ActiveEngine = engine;
                IsInSelectionMode = false;
            }
        }

        public void ScrollPages(int change)
        {
            CurrentPage += change;
        }

        private async void HandleThrottleChange(int throttle)
        {
            if (_activeEngine != null)
            {
                await _engineService.SetSpeed(_activeEngine.Id, throttle, _isDrivingForward);
            }
        }

        [UsedImplicitly]
        private void ChangeDirection(bool shouldBeDrivingForward)
        {
            IsDrivingForward = shouldBeDrivingForward;

            // HandleThrottleChange doesn't get notified if the value isn't changed, so it has to be done manually in that case
            // If we always did that, the HandleThrottleChange could be called twice. (once via RaiseAndSetIfChanged and once manually)
            if (Throttle == 0)
            {
                HandleThrottleChange(0);
            }
            else
            {
                Throttle = 0;
            }
        }

        [UsedImplicitly]
        private async void HandleFunction(ToggleButton button)
        {
            var functionNumber = (byte)(button.Tag ?? throw new InvalidOperationException());
            if (_activeEngine == null || button.IsChecked == null)
            {
                throw new InvalidOperationException();
            }

            await _engineService.SetFunction(_activeEngine.Id, functionNumber, (bool)button.IsChecked);
        }

        private async void Load((int Page, SortEnginesBy SortBy, string SortDirection, bool ShowHidden) args)
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
    }
}
