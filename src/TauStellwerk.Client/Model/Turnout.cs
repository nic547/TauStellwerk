﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Client.Model;

public partial class Turnout : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOn))]
    private State _state;

    [ObservableProperty]
    private string _name = string.Empty;

    public Turnout(TurnoutDto dto)
    {
        Name = dto.Name;
        Id = dto.Id;
        Address = dto.Address;
        Kind = dto.Kind;
        State = dto.State;
    }

    public Turnout()
    {
    }

    public int Id { get; }

    public int Address { get; set; }

    public TurnoutKind Kind { get; set; }

    public bool IsOn => State == State.On;

    public TurnoutDto ToDto()
    {
        return new TurnoutDto()
        {
            Name = Name,
            Address = Address,
            Id = Id,
            Kind = Kind,
            State = State,
        };
    }
}
