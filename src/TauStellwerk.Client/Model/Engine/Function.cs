using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Client.Model;

public class Function: ObservableObject
{
    private string _name;
    private State _state = State.Off;

    public Function(byte number, string name)
    {
        Number = number;
        _name = name;
    }

    public Function(byte number, string name, State state)
        : this(number, name)
    {
        State = state;
    }

    public byte Number { get; }


    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public State State
    {
        get => _state;
        internal set => SetProperty(ref _state, value);
    }

    public static ObservableCollection<Function> FromFunctionDtoList(IList<FunctionDto> functions)
    {
        return new ObservableCollection<Function>(functions.Select(f => new Function(f.Number, f.Name, f.State)));
    }

    public static List<FunctionDto> ToFunctionDtoList(ObservableCollection<Function> functions)
    {
        return functions.Select(f => new FunctionDto(f.Number, f.Name)).ToList();
    }

    public override string ToString() => $"F{Number} - {Name}";
}
