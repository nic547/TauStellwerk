// <copyright file="EngineControlViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels.Engine;

public partial class EngineControlViewModel : ViewModelBase
{
    private readonly EngineService _engineService;

    [ObservableProperty]
    private bool _isDrivingForward = true;

    [ObservableProperty]
    private int _throttle;

    public EngineControlViewModel(EngineFull engine, EngineService? engineService = null)
    {
        Engine = engine;

        _engineService = engineService ?? Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();

        PropertyChanged += HandlePropertyChanged;
    }

    public EngineControlViewModel()
        : this(new EngineFull()
        {
            Name = "TESTENGINE",
        })
    {
    }

    public EngineFull Engine { get; }

    public void OnClosing(object? sender, CancelEventArgs e)
    {
        _ = _engineService.ReleaseEngine(Engine.Id);
    }

    private async Task HandleThrottleChange(int throttle)
    {
        await _engineService.SetSpeed(Engine.Id, throttle, _isDrivingForward);
    }

    private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        switch (args.PropertyName)
        {
            case nameof(Throttle):
                _ = HandleThrottleChange(Throttle);
                break;
        }
    }

    [ICommand]
    private async Task ChangeDirection(string shouldBeDrivingForward)
    {
        IsDrivingForward = bool.Parse(shouldBeDrivingForward);

        // HandleThrottleChange doesn't get notified if the value isn't changed, so it has to be done manually in that case
        // If we always did that, the HandleThrottleChange could be called twice. (once via RaiseAndSetIfChanged and once manually)
        if (Throttle == 0)
        {
            await HandleThrottleChange(0);
        }
        else
        {
            Throttle = 0;
        }
    }

    [ICommand]
    private async Task Function(ToggleButton button)
    {
        var functionNumber = (byte)(button.Tag ?? throw new InvalidOperationException());
        if (button.IsChecked == null)
        {
            throw new InvalidOperationException();
        }

        await _engineService.SetFunction(Engine.Id, functionNumber, (bool)button.IsChecked);
    }

    [ICommand]
    private async Task EmergencyStop()
    {
        Throttle = 0;
        await _engineService.SetEStop(Engine.Id);
    }
}