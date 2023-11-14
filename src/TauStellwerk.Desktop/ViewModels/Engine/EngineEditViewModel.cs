// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Model.Engine;
using TauStellwerk.Client.Resources;
using TauStellwerk.Client.Services;
using TauStellwerk.Desktop.Services;

// ReSharper disable UnusedMember.Local
namespace TauStellwerk.Desktop.ViewModels;

public partial class EngineEditViewModel : ViewModelBase
{
    private readonly IAvaloniaViewService _viewService;
    private readonly EngineService _engineService;

    private readonly MemoryStream _imageStream = new();

    [ObservableProperty]
    private string _imageFilename = string.Empty;

    [ObservableProperty]
    private string _tagInputText = string.Empty;

    public EngineEditViewModel(EngineFull engine, EngineService? engineService = null, IAvaloniaViewService? viewService = null)
    {
        Engine = engine;

        _engineService = engineService ?? Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();
        _viewService = viewService ?? Locator.Current.GetService<IAvaloniaViewService>() ?? throw new InvalidOperationException();

        // Intentionally done once, I don't want the new name to show up until the engine is saved.
        WindowTitle = $"{engine.Name} - {Resources.Edit}";
    }

    public delegate void HandleClosingRequested();

    public event HandleClosingRequested? ClosingRequested;

    public EngineFull Engine { get; }

    public string WindowTitle { get; init; }

    public async void HandleWindowClosing(object? sender, EventArgs e)
    {
        await _engineService.ReleaseEngine(Engine.Id);
    }

    [RelayCommand]
    private async Task Save()
    {
        var updatedEngine = await _engineService.AddOrUpdateEngine(Engine);

        if (_imageStream.Length > 0)
        {
            await _engineService.UpdateEngineImage(updatedEngine, _imageStream, ImageFilename);
        }

        ClosingRequested?.Invoke();
    }

    [RelayCommand]
    private async Task Delete()
    {
        var deleteResult = await _engineService.TryDeleteEngine(Engine);
        if (deleteResult.Success)
        {
            ClosingRequested?.Invoke();
            return;
        }

        _viewService.ShowMessageBox("Failed to delete engine", $"Failed to delete engine: {deleteResult.Error}", this);
    }

    [RelayCommand]
    private void Cancel()
    {
        ClosingRequested?.Invoke();
    }

    [RelayCommand]
    private void Copy()
    {
        _viewService.ShowEngineEditView(Engine.CreateCopy(), this);
    }

    [RelayCommand]
    private async Task SelectImage()
    {
        FilePickerOpenOptions options = new()
        {
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                Util.FilePickerFileTypes.ImageAllExtended,
            },
        };

        var file = await _viewService.ShowFilePicker(this, options);

        if (file != null)
        {
            await using var stream = await file.OpenReadAsync();
            await stream.CopyToAsync(_imageStream);
            ImageFilename = file.Name;
        }
    }

    [RelayCommand]
    private void AddTag()
    {
        if (TagInputText != string.Empty)
        {
            Engine.Tags.Add(TagInputText);
            TagInputText = string.Empty;
        }
    }

    [RelayCommand]
    private void RemoveTag(string tag)
    {
        Engine.Tags.Remove(tag);
    }

    [RelayCommand]
    private void AddFunction()
    {
        var lastFunctionNumber = Engine.Functions.LastOrDefault()?.Number;
        lastFunctionNumber++;
        lastFunctionNumber ??= 0;
        Engine.Functions.Add(new Function((byte)lastFunctionNumber, string.Empty, 0));
    }

    [RelayCommand]
    private void RemoveLastFunction()
    {
        var last = Engine.Functions.LastOrDefault();
        if (last != null)
        {
            Engine.Functions.Remove(last);
        }
    }
}
