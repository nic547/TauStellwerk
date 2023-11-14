// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model.Engine;

namespace TauStellwerk.Client.Model;

public partial class EngineFull : EngineOverview
{
    [ObservableProperty]
    private ushort _address;

    [ObservableProperty]
    private int _topSpeed;
    private Direction _direction;
    private int _throttle;

    public EngineFull(EngineFullDto engine)
        : base(engine)
    {
        Name = engine.Name;
        Functions = new ObservableCollection<Function>(Function.FromFunctionDtoList(engine.Functions.OrderBy(x => x.Number).ToList()));
        Address = engine.Address;
        TopSpeed = engine.TopSpeed;
        Throttle = engine.Throttle;
        Direction = engine.Direction;
    }

    public EngineFull()
        : base()
    {
        Functions = new();
    }

    public ObservableCollection<Function> Functions { get; init; }

    public Direction Direction
    {
        get => _direction;
        internal set => SetProperty(ref _direction, value);
    }

    public int Throttle
    {
        get => _throttle;
        internal set => SetProperty(ref _throttle, value);
    }

    public static EngineFull? Create(EngineFullDto? engineDto)
    {
        return engineDto is null ? null : new EngineFull(engineDto);
    }

    public EngineFull CreateCopy()
    {
        var copy = new EngineFull()
        {
            Name = Name + " (Copy)",
            Tags = new ObservableCollection<string>(Tags),
            Functions = new ObservableCollection<Function>(Functions.Select(f => new Function(f.Number, f.Name, f.Duration))),
            TopSpeed = TopSpeed,
        };

        return copy;
    }

    public EngineFullDto ToDto()
    {
        return new EngineFullDto
        {
            Address = Address,
            Created = Created,

            Functions = Function.ToFunctionDtoList(Functions),
            Id = Id,
            Images = EngineImage.ToDtos(Images),
            IsHidden = IsHidden,
            LastUsed = LastUsed,
            Name = Name,
            Tags = Tags.ToList(),
            TopSpeed = TopSpeed,
        };
    }
}
