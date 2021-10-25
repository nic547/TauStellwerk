// <copyright file="EngineEditViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ReactiveUI;
using Splat;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.ViewModels.Engine;

public class EngineEditViewModel : ViewModelBase
{
    private readonly EngineService _engineService;
    private string _tagInputText = string.Empty;

    public EngineEditViewModel(EngineFull engine, EngineService? engineService = null)
    {
        Engine = engine;

        _engineService = engineService ?? Locator.Current.GetService<EngineService>() ?? throw new InvalidOperationException();

        SaveCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(HandleSave);
        CancelCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(HandleCancel);
        AddTagCommand = ReactiveCommand.Create<Unit, Unit>(HandleAddTag);
        RemoveTagCommand = ReactiveCommand.Create<string, Unit>(HandleRemoveTag);
        AddFunctionCommand = ReactiveCommand.Create<Unit, Unit>(HandleAddFunction);
        RemoveLastFunctionCommand = ReactiveCommand.Create<Unit, Unit>(HandleRemoveLastFunction);
    }

    public delegate void HandleClosingRequested();

    public event HandleClosingRequested? ClosingRequested;

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public ReactiveCommand<Unit, Unit> AddTagCommand { get; }

    [UsedImplicitly]
    public ReactiveCommand<string, Unit> RemoveTagCommand { get; }

    public ReactiveCommand<Unit, Unit> AddFunctionCommand { get; }

    public ReactiveCommand<Unit, Unit> RemoveLastFunctionCommand { get; }

    public EngineFull Engine { get; }

    public string TagInputText
    {
        get => _tagInputText;
        set => this.RaiseAndSetIfChanged(ref _tagInputText, value);
    }

    public async void HandleWindowClosing(object? sender, EventArgs e)
    {
        await _engineService.ReleaseEngine(Engine.Id);
    }

    private async Task<Unit> HandleSave(Unit arg)
    {
        await _engineService.AddOrUpdateEngine(Engine);
        ClosingRequested?.Invoke();
        return Unit.Default;
    }

    private Task<Unit> HandleCancel(Unit arg)
    {
        ClosingRequested?.Invoke();
        return Task.FromResult(Unit.Default);
    }

    private Unit HandleAddTag(Unit arg)
    {
        if (TagInputText != string.Empty)
        {
            Engine.Tags.Add(TagInputText);
            TagInputText = string.Empty;
        }

        return Unit.Default;
    }

    private Unit HandleRemoveTag(string tag)
    {
        Engine.Tags.Remove(tag);

        return Unit.Default;
    }

    private Unit HandleAddFunction(Unit unit)
    {
        var lastFunctionNumber = Engine.Functions.LastOrDefault()?.Number;
        lastFunctionNumber++;
        lastFunctionNumber ??= 0;
        Engine.Functions.Add(new FunctionDto((byte)lastFunctionNumber, string.Empty));
        return Unit.Default;
    }

    private Unit HandleRemoveLastFunction(Unit unit)
    {
        var last = Engine.Functions.LastOrDefault();
        if (last != null)
        {
            Engine.Functions.Remove(last);
        }

        return Unit.Default;
    }
}