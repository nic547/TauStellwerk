// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

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
