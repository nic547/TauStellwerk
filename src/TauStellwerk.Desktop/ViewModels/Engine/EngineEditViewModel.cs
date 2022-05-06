// <copyright file="EngineEditViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Client;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels.Engine;

public partial class EngineEditViewModel : ViewModelBase
{
    private readonly IViewService _viewService;
    private readonly EngineService _engineService;

    [ObservableProperty]
    private string _tagInputText = string.Empty;

    public EngineEditViewModel(EngineFull engine, EngineService? engineService = null, AvaloniaViewService? viewService = null)
    {
        Engine = engine;

        _engineService = engineService ?? Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();
        _viewService = viewService ?? Locator.Current.GetService<IViewService>() ?? throw new InvalidOperationException();
    }

    public delegate void HandleClosingRequested();

    public event HandleClosingRequested? ClosingRequested;

    public EngineFull Engine { get; }

    public async void HandleWindowClosing(object? sender, EventArgs e)
    {
        await _engineService.ReleaseEngine(Engine.Id);
    }

    [ICommand]
    private async Task Save()
    {
        await _engineService.AddOrUpdateEngine(Engine);
        ClosingRequested?.Invoke();
    }

    [ICommand]
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

    [ICommand]
    private void Cancel()
    {
        ClosingRequested?.Invoke();
    }

    [ICommand]
    private void AddTag()
    {
        if (TagInputText != string.Empty)
        {
            Engine.Tags.Add(TagInputText);
            TagInputText = string.Empty;
        }
    }

    [ICommand]
    private void RemoveTag(string tag)
    {
        Engine.Tags.Remove(tag);
    }

    [ICommand]
    private void AddFunction()
    {
        var lastFunctionNumber = Engine.Functions.LastOrDefault()?.Number;
        lastFunctionNumber++;
        lastFunctionNumber ??= 0;
        Engine.Functions.Add(new Function((byte)lastFunctionNumber, string.Empty, 0));
    }

    [ICommand]
    private void RemoveLastFunction()
    {
        var last = Engine.Functions.LastOrDefault();
        if (last != null)
        {
            Engine.Functions.Remove(last);
        }
    }
}