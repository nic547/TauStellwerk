// <copyright file="EngineControlViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using JetBrains.Annotations;
using ReactiveUI;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels.Engine
{
    public class EngineControlViewModel : ViewModelBase
    {
        private readonly EngineService _engineService;
        private bool _isDrivingForward = true;
        private int _throttle;
        private List<FunctionDto> _sortedFunctions = new();

        public EngineControlViewModel(EngineFullDto engine, EngineService? engineService = null)
        {
            Engine = engine;

            _engineService = engineService ?? Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();

            EmergencyStopCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(HandleEStop);

            this.WhenAnyValue(v => v.Throttle)
                .Select(x => _ = HandleThrottleChange(x))
                .Subscribe();
            this.WhenAnyValue(p => p.Engine.Functions)
                .Select(f => SortedFunctions = f.OrderBy(o => o.Number).ToList())
                .Subscribe();
        }

        public EngineControlViewModel()
            : this(new EngineFullDto
            {
                Name = "TESTENGINE",
            })
        {
        }

        public EngineFullDto Engine { get; }

        public List<FunctionDto> SortedFunctions
        {
            get => _sortedFunctions;
            private set => this.RaiseAndSetIfChanged(ref _sortedFunctions, value);
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

        public ReactiveCommand<Unit, Unit> EmergencyStopCommand { get; }

        public void OnClosing()
        {
            _ = _engineService.ReleaseEngine(Engine.Id);
        }

        private async Task HandleThrottleChange(int throttle)
        {
            await _engineService.SetSpeed(Engine.Id, throttle, _isDrivingForward);
        }

        [UsedImplicitly]
        private void ChangeDirection(bool shouldBeDrivingForward)
        {
            IsDrivingForward = shouldBeDrivingForward;

            // HandleThrottleChange doesn't get notified if the value isn't changed, so it has to be done manually in that case
            // If we always did that, the HandleThrottleChange could be called twice. (once via RaiseAndSetIfChanged and once manually)
            if (Throttle == 0)
            {
                _ = HandleThrottleChange(0);
            }
            else
            {
                Throttle = 0;
            }
        }

        [UsedImplicitly]
        private async Task HandleFunction(ToggleButton button)
        {
            var functionNumber = (byte)(button.Tag ?? throw new InvalidOperationException());
            if (button.IsChecked == null)
            {
                throw new InvalidOperationException();
            }

            await _engineService.SetFunction(Engine.Id, functionNumber, (bool)button.IsChecked);
        }

        private async Task<Unit> HandleEStop(Unit arg)
        {
            Throttle = 0;
            await _engineService.SetEStop(Engine.Id);
            return Unit.Default;
        }
    }
}
