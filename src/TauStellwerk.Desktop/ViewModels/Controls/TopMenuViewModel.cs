// <copyright file="TopMenuViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Desktop.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class TopMenuViewModel : ViewModelBase
{
    private readonly IAvaloniaViewService _viewService;

    public TopMenuViewModel(IAvaloniaViewService? viewService = null)
    {
        _viewService = viewService ?? Locator.Current.GetService<IAvaloniaViewService>() ?? throw new InvalidOperationException();
    }

    public StopButtonControlViewModel StopButtonVm { get; } = new();

    [RelayCommand]
    protected virtual void OpenEngineList()
    {
        _viewService.ShowEngineSelectionView(this);
    }

    [RelayCommand]
    protected virtual void OpenSettings()
    {
        _viewService.ShowSettingsView(this);
    }

    [RelayCommand]
    protected virtual void OpenTurnoutList()
    {
        _viewService.ShowTurnoutsWindow();
    }
}