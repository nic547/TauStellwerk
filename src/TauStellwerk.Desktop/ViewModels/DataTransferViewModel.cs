// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class DataTransferViewModel : ViewModelBase, IDisposable
{
    private readonly DataTransferService _service;

    [ObservableProperty]
    private ObservableCollection<string> _backups = [];

    public DataTransferViewModel(DataTransferService? service = null)
    {
        _service = service ?? Locator.Current.GetRequiredService<DataTransferService>();
        _service.BackupCreated += OnBackupCreated;

        _ = InitalLoad();
    }

    private void OnBackupCreated(object? sender, string s)
    {
        Backups.Add(s);
    }

    [RelayCommand]
    private async Task CreateNewBackup()
    {
        await _service.StartBackup();
    }

    private async Task InitalLoad()
    {
        var result = await _service.GetBackups();
        Backups = new ObservableCollection<string>(result);
    }

    public void Dispose()
    {
        _service.BackupCreated -= OnBackupCreated;
    }
}
