// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Base.Dto;
using TauStellwerk.Client.Services;
using TauStellwerk.Desktop.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class DataTransferViewModel : ViewModelBase, IDisposable
{
    private readonly DataTransferService _service;
    private readonly IAvaloniaViewService _viewService;

    [ObservableProperty]
    private ObservableCollection<BackupInfoDto> _backups = [];

    public DataTransferViewModel(DataTransferService? service = null, IAvaloniaViewService? viewService = null)
    {
        _service = service ?? Locator.Current.GetRequiredService<DataTransferService>();
        _viewService = viewService ?? Locator.Current.GetRequiredService<IAvaloniaViewService>();

        _service.BackupCreated += OnBackupCreated;
        _service.BackupDeleted += OnBackupDeleted;

        _ = InitalLoad();
    }

    private void OnBackupDeleted(object? sender, string fileName)
    {
        var existingBackup = Backups.SingleOrDefault(b => b.FileName == fileName);
        if (existingBackup != null)
        {
            Backups.Remove(existingBackup);
        }
    }

    private void OnBackupCreated(object? sender, BackupInfoDto newBackup)
    {
        var existingBackup = Backups.SingleOrDefault(b => b.FileName == newBackup.FileName);
        if (existingBackup != null)
        {
            Backups.Remove(existingBackup);
        }

        Backups.Insert(0, newBackup);
    }

    [RelayCommand]
    private async Task CreateNewBackup()
    {
        await _service.StartBackup();
    }

    [RelayCommand]
    private async Task DownloadBackup(string fileName)
    {
        FilePickerSaveOptions options = new()
        {
            SuggestedFileName = fileName
        };
        var storageFile = await _viewService.ShowSaveFilePicker(this, options);

        if (storageFile is null)
        {
            return;
        }

        await _service.DownloadBackup(fileName, storageFile.Path);
    }

    [RelayCommand]
    private async Task DeleteBackup(string fileName)
    {
        await _service.DeleteBackup(fileName);
    }

    private async Task InitalLoad()
    {
        var result = await _service.GetBackups();
        Backups = new ObservableCollection<BackupInfoDto>(result);
    }

    public void Dispose()
    {
        _service.BackupCreated -= OnBackupCreated;
    }
}
