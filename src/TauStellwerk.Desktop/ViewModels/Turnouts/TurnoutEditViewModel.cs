// <copyright file="TurnoutEditViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Base;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class TurnoutEditViewModel
{
    private ITurnoutService _turnoutService;

    public TurnoutEditViewModel(Turnout turnout, ITurnoutService? turnoutService = null)
    {
        Turnout = turnout;
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
}