// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Desktop.Services;
using TauStellwerk.Desktop.Services.WindowSettingService;

namespace TauStellwerk.Desktop.ViewModels;

public partial class TopMenuViewModel : ViewModelBase, IDisposable
{
    private readonly IAvaloniaViewService _viewService;
    private readonly IWindowSettingService _windowSettingService;

    private string _windowType = string.Empty;

    [ObservableProperty]
    private bool _useLargeButton = false;

    public TopMenuViewModel(IAvaloniaViewService? viewService = null)
    {
        _viewService = viewService ?? Locator.Current.GetService<IAvaloniaViewService>() ?? throw new InvalidOperationException();
        _windowSettingService = Locator.Current.GetService<IWindowSettingService>() ?? throw new InvalidOperationException();

        _windowSettingService.UseLargeButtonChanged += LargeButtonChangedHandler;
    }

    private void LargeButtonChangedHandler(object? sender, (string WindowType, bool UseLargeButton) args)
    {
        if (args.WindowType == _windowType)
        {
            UseLargeButton = args.UseLargeButton;
        }
    }

    public StopButtonControlViewModel StopButtonVm { get; } = Locator.Current.GetRequiredService<StopButtonControlViewModel>();

    public void UpdateWindowType(string windowType)
    {
        _windowType = windowType;
        UseLargeButton = _windowSettingService.LoadUseLargeButton(windowType) ?? false;
    }

    [RelayCommand]
    protected void ToggleLargeButton()
    {
        _windowSettingService.SaveUseLargeButton(_windowType, !UseLargeButton);
    }

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

    [RelayCommand]
    protected virtual void OpenProgrammingWindow()
    {
        _viewService.ShowProgrammingWindow();
    }

    [RelayCommand]
    protected virtual void OpenDataTransferWindow()
    {
        _viewService.ShowDataTransferWindow();
    }

    public void Dispose()
    {
        _windowSettingService.UseLargeButtonChanged -= LargeButtonChangedHandler;
        GC.SuppressFinalize(this);
    }
}
