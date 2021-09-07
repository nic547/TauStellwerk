// <copyright file="EngineControlViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

        public EngineControlViewModel(EngineFullDto engine, EngineService? engineService = null)
        {
            Engine = engine;

            _engineService = engineService ??
                             Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();

            this.WhenAnyValue(v => v.Throttle).Throttle(TimeSpan.FromMilliseconds(100)).Subscribe(HandleThrottleChange);
        }

        public EngineControlViewModel()
            : this(new EngineFullDto
            {
                Name = "TESTENGINE",
            })
        {
        }

        public EngineFullDto Engine { get; }

        public List<FunctionDto> SortedFunctions => Engine.Functions.OrderBy(f => f.Number).ToList();

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

        private async void HandleThrottleChange(int throttle)
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
            if (button.IsChecked == null)
            {
                throw new InvalidOperationException();
            }

            await _engineService.SetFunction(Engine.Id, functionNumber, (bool)button.IsChecked);
        }
    }
}
