// <copyright file="MainWindowViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Client.Resources;
using TauStellwerk.Client.Services;
using TauStellwerk.Desktop.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IAvaloniaViewService _viewService;

    [ObservableProperty]
    private ThemeVariant _themeMode = ThemeVariant.Default;

    public MainWindowViewModel(ISettingsService? settingsService = null, AvaloniaViewService? viewService = null)
    {
        _settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>() ?? throw new InvalidOperationException();
        _viewService = viewService ?? Locator.Current.GetService<IAvaloniaViewService>() ?? throw new InvalidOperationException();

        var settings = _settingsService.GetSettings().Result;

        ThemeMode = ParseThemeVariant(settings.Theme);
        Languages.SetUILanguage(settings.Language);

        _settingsService.SettingsChanged += (updatedSetting) =>
    {
        ThemeMode = ParseThemeVariant(updatedSetting.Theme);
        Languages.SetUILanguage(updatedSetting.Language);
    };
    }

    private static ThemeVariant ParseThemeVariant(string name)
    {
        return name switch
        {
            "Dark" => ThemeVariant.Dark,
            "Light" => ThemeVariant.Light,
            _ => ThemeVariant.Default,
        };
    }

    [RelayCommand]
    private void OpenEngineList()
    {
        _viewService.ShowEngineSelectionView(this);
    }

    [RelayCommand]
    private void OpenSettings()
    {
        _viewService.ShowSettingsView(this);
    }

    [RelayCommand]
    private void OpenTurnoutList()
    {
        _viewService.ShowTurnoutsWindow();
    }
}