// <copyright file="SettingsViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Resources;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private MutableSettings? _settings;

    public SettingsViewModel(SettingsService? settingsService = null)
    {
        _settingsService = settingsService ??
                           Locator.Current.GetService<ISettingsService>() ?? throw new InvalidOperationException();
        _ = LoadSettings();
    }

    public delegate void HandleClosingRequested();

    public event HandleClosingRequested? ClosingRequested;

    public static List<string> AvailableLanguages => Languages.LanguageNames;

    public static string ApplicationInformation =>
        $"TauStellwerk {ThisAssembly.AssemblyInformationalVersion} (.NET {Environment.Version})";

    public string[] AvailableThemes { get; } = { ThemeVariant.Light.ToString(), ThemeVariant.Dark.ToString() };

    [RelayCommand]
    public async Task Save()
    {
        if (Settings != null)
        {
            await _settingsService.SetSettings(Settings);
        }

        ClosingRequested?.Invoke();
    }

    [RelayCommand]
    public void Cancel()
    {
        ClosingRequested?.Invoke();
    }

    private async Task LoadSettings()
    {
        Settings = await _settingsService.GetMutableSettingsCopy();
    }
}