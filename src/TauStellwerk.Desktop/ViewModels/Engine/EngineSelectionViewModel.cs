// <copyright file="EngineSelectionViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels.Engine;

public partial class EngineSelectionViewModel : ViewModelBase
{
    private const int EnginesPerPage = 20;

    private readonly EngineService _engineService;
    private readonly IViewService _viewService;

    [ObservableProperty]
    private bool _showHiddenEngines;

    [ObservableProperty]
    private SortEnginesBy _currentEngineSortMode = SortEnginesBy.LastUsed;

    [ObservableProperty]
    private string _currentEngineSortDirection = "DESC";

    [ObservableProperty]
    private int _currentPage;

    public EngineSelectionViewModel(EngineService? engineService = null, IViewService? viewService = null)
    {
        _engineService = engineService ??
                         Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();
        _viewService = viewService ??
                       Locator.Current.GetService<IViewService>() ?? throw new InvalidOperationException();

        PropertyChanged += HandlePropertyChanged;
        _ = Load();
    }

    public delegate void HandleClosingRequested();

    public event HandleClosingRequested? ClosingRequested;

    public static SortEnginesBy[] EngineSortModes => Enum.GetValues<SortEnginesBy>();

    public static string[] EngineSortDirections => new[] { "ASC", "DESC" };

    public ObservableCollection<EngineOverview> Engines { get; } = new();

    public bool CanScrollForwards => Engines.Count == EnginesPerPage;

    public bool CanScrollBackwards => CurrentPage > 0;

    public void ScrollPages(int change)
    {
        CurrentPage += change;
    }

    private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        switch (args.PropertyName)
        {
            case nameof(CurrentPage):
            case nameof(CurrentEngineSortDirection):
            case nameof(ShowHiddenEngines):
            case nameof(CurrentEngineSortMode):
                _ = Load();
                break;
        }
    }

    [ICommand]
    private async Task ControlEngine(int id)
    {
        var engine = await _engineService.AcquireEngine(id);
        if (engine != null)
        {
            _viewService.ShowEngineControlView(engine);
            ClosingRequested?.Invoke();
        }
        else
        {
            _viewService.ShowMessageBox(
                "Cannot acquire engine",
                "Cannot control the engine because it seems to be in use by someone else",
                this);
        }
    }

    private async Task Load()
    {
        var sortDescending = _currentEngineSortDirection == "DESC";
        var engines = await _engineService.GetEngines(_currentPage, _currentEngineSortMode, sortDescending, _showHiddenEngines);

        Engines.Clear();
        foreach (var engine in engines)
        {
            Engines.Add(engine);
        }

        OnPropertyChanged(nameof(CanScrollForwards));
        OnPropertyChanged(nameof(CanScrollBackwards));
    }

    [ICommand]
    private async Task EditEngine(int id)
    {
        var engine = await _engineService.AcquireEngine(id);
        if (engine != null)
        {
            _viewService.ShowEngineEditView(engine, this);
        }
        else
        {
            _viewService.ShowMessageBox(
                "Cannot acquire engine",
                "Cannot edit the engine because it seems to be in use by someone else",
                this);
        }
    }

    [ICommand]
    private void CreateEngine()
    {
        var engine = new EngineFull();
        _viewService.ShowEngineEditView(engine, this);
    }
}