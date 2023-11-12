// <copyright file="TurnoutEditViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Base;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class TurnoutEditViewModel
{
    private readonly IViewService _viewService;
    private readonly ITurnoutService _turnoutService;

    public TurnoutEditViewModel(Turnout turnout, ITurnoutService? turnoutService = null, IViewService? viewService = null)
    {
        Turnout = turnout;

        _viewService = viewService ?? Locator.Current.GetService<IViewService>() ?? throw new InvalidOperationException();
        _turnoutService = turnoutService ?? Locator.Current.GetService<ITurnoutService>() ?? throw new InvalidOperationException();
    }

    public delegate void HandleClosingRequested();

    public event HandleClosingRequested? ClosingRequested;

    public Turnout Turnout { get; init; }

    public IList<string> TurnoutKinds { get; } = Enum.GetNames(typeof(TurnoutKind));

    [RelayCommand]
    private async Task Save()
    {
        await _turnoutService.AddOrUpdate(Turnout);

        ClosingRequested?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        ClosingRequested?.Invoke();
    }

    [RelayCommand]
    private async Task Delete()
    {
        var result = await _turnoutService.Delete(Turnout);
        if (result.Success)
        {
            ClosingRequested?.Invoke();
        }
        else
        {
            _viewService.ShowMessageBox("Failed to delete turnout", $"Failed to delete turnout: \"{result.Error}\"", this);
        }
    }
}