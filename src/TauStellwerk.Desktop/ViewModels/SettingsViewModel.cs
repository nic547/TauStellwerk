// <copyright file="SettingsViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private MutableSettings? _settings;

    public SettingsViewModel(SettingsService? settingsService = null)
    {
        _settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>() ?? throw new InvalidOperationException();
        _ = LoadSettings();
    }

    public delegate void HandleClosingRequested();

    public event HandleClosingRequested? ClosingRequested;

    [ICommand]
    public async Task Save()
    {
        if (_settings != null)
        {
            await _settingsService.SetSettings(_settings);
        }

        ClosingRequested?.Invoke();
    }

    [ICommand]
    public void Cancel()
    {
        ClosingRequested?.Invoke();
    }

    private async Task LoadSettings()
    {
        Settings = await _settingsService.GetMutableSettingsCopy();
    }
}