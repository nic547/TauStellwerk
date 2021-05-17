// <copyright file="EngineViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PiStellwerk.Data;
using PiStellwerk.Desktop.Services;
using ReactiveUI;
using Splat;

namespace PiStellwerk.Desktop.ViewModels
{
    public class EngineViewModel : ViewModelBase
    {
        private readonly EngineService _engineService;
        private Engine? _activeEngine;
        private bool _isInSelectionMode = true;

        private bool _isDrivingForward = true;

        private int _throttle;

        public EngineViewModel(EngineService? engineService = null)
        {
            _engineService = engineService ?? Locator.Current.GetService<EngineService>();
            Load();

            this.WhenAnyValue(v => v.Throttle).Throttle(TimeSpan.FromMilliseconds(50)).Subscribe(HandleThrottleChange);
        }

        public ObservableCollection<Engine> Engines { get; } = new();

        public Engine? ActiveEngine
        {
            get => _activeEngine;
            set => this.RaiseAndSetIfChanged(ref _activeEngine, value);
        }

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

        public async Task SelectEngine(int id)
        {
            var engine = await _engineService.AcquireEngine(id);
            if (engine != null)
            {
                ActiveEngine = engine;
                IsInSelectionMode = false;
            }
        }

        public async void HandleThrottleChange(int throttle)
        {
            if (_activeEngine != null)
            {
                await _engineService.SetSpeed(_activeEngine.Id, throttle, _isDrivingForward);
            }
        }

        private void ChangeDirection(bool shouldBeDrivingForward)
        {
            IsDrivingForward = shouldBeDrivingForward;
            Throttle = 0;
        }

        private async void Load()
        {
            var engines = await _engineService.GetEngines();

            foreach (var engine in engines)
            {
                Engines.Add(engine);
            }
        }
    }
}
