// <copyright file="Function.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Client.Model;

public class Function : ObservableObject
{
    private string _name;
    private int _duration;
    private State _state = State.Off;

    private Timer? _timer;

    public Function(byte number, string name, int duration)
    {
        Number = number;
        Duration = duration;
        _name = name;
    }

    public Function(byte number, string name, int duration,  State state)
        : this(number, name, duration)
    {
        State = state;
    }

    public byte Number { get; }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public int Duration
    {
        get => _duration;
        set => SetProperty(ref _duration, value);
    }

    public State State
    {
        get => _state;
        internal set
        {
            SetProperty(ref _state, value);
            OnPropertyChanged(nameof(IsOn));
            CheckAndHandleMomentaryFunction();
        }
    }

    public bool IsOn => State == State.On;

    public static ObservableCollection<Function> FromFunctionDtoList(IList<FunctionDto> functions)
    {
        return new ObservableCollection<Function>(functions.Select(f => new Function(f.Number, f.Name, f.Duration, f.State)));
    }

    public static List<FunctionDto> ToFunctionDtoList(ObservableCollection<Function> functions)
    {
        return functions.Select(f => new FunctionDto(f.Number, f.Name, f.Duration)).ToList();
    }

    public void CheckAndHandleMomentaryFunction()
    {
        if (Duration > 0 && State == State.On)
        {
            if (_timer == null)
            {
                _timer = new Timer
                {
                    AutoReset = false,
                };
                _timer.Elapsed += (_, _) => { State = State.Off; };
            }

            _timer.Interval = Duration;
            _timer.Start();
        }
    }

    public override string ToString() => $"F{Number} - {Name}";
}
