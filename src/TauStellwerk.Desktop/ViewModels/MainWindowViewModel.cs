﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using Splat;
using TauStellwerk.Client.Model.Settings;
using TauStellwerk.Client.Services;
using TauStellwerk.Desktop.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class MainWindowViewModel : TopMenuViewModel
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private ThemeVariant _themeMode = ThemeVariant.Default;

    public MainWindowViewModel(ISettingsService? settingsService = null, AvaloniaViewService? viewService = null)
        : base(viewService)
    {
        _settingsService = settingsService ??
                           Locator.Current.GetService<ISettingsService>() ?? throw new InvalidOperationException();

        var settings = _settingsService.GetSettings().Result;

        _settingsService.SettingsChanged += HandleSettingsChange;
        HandleSettingsChange(settings);
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

    private void HandleSettingsChange(ImmutableSettings settings)
    {
        ThemeMode = ParseThemeVariant(settings.Theme);
    }
}
