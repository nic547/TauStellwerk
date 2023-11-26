// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels;

public partial class EngineSelectionViewModel : ViewModelBase, IDisposable
{
    private const int EnginesPerPage = 20;

    private readonly EngineService _engineService;
    private readonly IViewService _viewService;

    private readonly object _collectionLock = new();

    [ObservableProperty]
    private string _currentSearchTerm = string.Empty;

    [ObservableProperty]
    private bool _showHiddenEngines;

    [ObservableProperty]
    private SortEnginesBy _currentEngineSortMode = SortEnginesBy.LastUsed;

    [ObservableProperty]
    private string _currentEngineSortDirection = "DESC";

    [ObservableProperty]
    private int _currentPage = 1;

    public EngineSelectionViewModel(EngineService? engineService = null, IViewService? viewService = null)
    {
        _engineService = engineService ??
                         Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();
        _viewService = viewService ??
                       Locator.Current.GetService<IViewService>() ?? throw new InvalidOperationException();

        PropertyChanged += async (o, args) => await HandlePropertyChanged(o, args);
        _engineService.EngineChanged += OnEngineChanged;

        _ = Load();
    }

    public delegate void HandleClosingRequested();

    public event HandleClosingRequested? ClosingRequested;

    public event EventHandler? ResetScroll;

    public static SortEnginesBy[] EngineSortModes => Enum.GetValues<SortEnginesBy>();

    public static string[] EngineSortDirections => new[] { "ASC", "DESC" };

    public ObservableCollection<EngineOverview> Engines { get; } = [];

    public bool CanScrollForwards => Engines.Count == EnginesPerPage;

    public bool CanScrollBackwards => CurrentPage > 1;

    public void ScrollPages(int change)
    {
        CurrentPage += change;
    }

    public void Dispose()
    {
        _engineService.EngineChanged -= OnEngineChanged;
        GC.SuppressFinalize(this);
    }

    private async Task HandlePropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        switch (args.PropertyName)
        {
            case nameof(CurrentPage):
                await Load();
                ResetScroll?.Invoke(this, EventArgs.Empty);
                break;

            case nameof(CurrentEngineSortDirection):
            case nameof(ShowHiddenEngines):
            case nameof(CurrentEngineSortMode):
            case nameof(CurrentSearchTerm):
                _ = Load();
                break;
            default:
                break;
        }
    }

    [RelayCommand]
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

    private void OnEngineChanged(object? sender, EngineFull changedEngine)
    {
        lock (_collectionLock)
        {
            for (var i = 0; i < Engines.Count; i++)
            {
                if (Engines[i].Id == changedEngine.Id)
                {
                    Engines[i] = changedEngine;
                    return;
                }
            }
        }
    }

    private async Task Load()
    {
        var sortDescending = CurrentEngineSortDirection == "DESC";
        var engines = await _engineService.GetEngines(CurrentSearchTerm, CurrentPage - 1, CurrentEngineSortMode, sortDescending, ShowHiddenEngines);

        lock (_collectionLock)
        {
            Engines.Clear();
            foreach (var engine in engines)
            {
                Engines.Add(engine);
            }
        }

        OnPropertyChanged(nameof(CanScrollForwards));
        OnPropertyChanged(nameof(CanScrollBackwards));
    }

    [RelayCommand]
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

    [RelayCommand]
    private void CreateEngine()
    {
        var engine = new EngineFull();
        _viewService.ShowEngineEditView(engine, this);
    }

    [RelayCommand]
    private void SetSearchTerm(string searchTerm)
    {
        CurrentSearchTerm = searchTerm;
    }
}
