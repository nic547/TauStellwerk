// <copyright file="TurnoutsViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;
using TauStellwerk.Desktop.Services;

namespace TauStellwerk.Desktop.ViewModels;

public sealed partial class TurnoutsViewModel : ViewModelBase, IDisposable
{
    private readonly IAvaloniaViewService _viewService;
    private readonly ITurnoutService _turnoutService;

    [ObservableProperty]
    private int _currentPage;

    public TurnoutsViewModel(TurnoutService? turnoutService = null, IAvaloniaViewService? viewService = null)
    {
        _turnoutService = turnoutService ?? Locator.Current.GetService<ITurnoutService>() ?? throw new InvalidOperationException();
        _viewService = viewService ?? Locator.Current.GetService<IAvaloniaViewService>() ?? throw new InvalidOperationException();

        PropertyChanged += HandlePageChange;

        _turnoutService.TurnoutStateChanged += HandleTurnoutStateChanged;

        LoadTurnouts();
    }

    public ObservableCollection<Turnout> Turnouts { get; set; } = new();

    public void Dispose()
    {
        _turnoutService.TurnoutStateChanged -= HandleTurnoutStateChanged;
    }

    private void HandleTurnoutStateChanged(object? sender, TurnoutStateChangedEventArgs e)
    {
        foreach (var turnout in Turnouts.Where(t => t.Address == e.Address))
        {
            turnout.State = e.State;
        }
    }

    private void HandlePageChange(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CurrentPage))
        {
            LoadTurnouts();
        }
    }

    private async void LoadTurnouts()
    {
        Turnouts.Clear();
        var turnouts = await _turnoutService.GetList(CurrentPage);
        foreach (var turnout in turnouts)
        {
            Turnouts.Add(turnout);
        }
    }

    [RelayCommand]
    private void CreateTurnout()
    {
        var turnout = new Turnout();
        _viewService.ShowTurnoutEditWindow(turnout, this);
    }

    [RelayCommand]
    private void ToggleTurnout(Turnout turnout)
    {
        _turnoutService.ToggleState(turnout);
    }

    [RelayCommand]
    private void EditTurnout(Turnout turnout)
    {
        _viewService.ShowTurnoutEditWindow(turnout, this);
    }
}