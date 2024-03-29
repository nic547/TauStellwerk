﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;

namespace TauStellwerk.CommandStations;

public class EngineState
{
    public EngineState(int numberOfEngines)
    {
        FunctionStates = Enumerable.Repeat(State.Off, numberOfEngines).ToList();
    }

    public int Throttle { get; set; }

    public Direction Direction { get; set; }

    public List<State> FunctionStates { get; set; }

    public void UpdateEngineFullDto(ref EngineFullDto dto)
    {
        if (dto.Functions.Count != FunctionStates.Count)
        {
            throw new InvalidOperationException("EngineFullDto cannot be updated because the number of functions does not match");
        }

        var orderedFunctions = dto.Functions.OrderBy(f => f.Number).ToList();

        dto.Throttle = Throttle;
        dto.Direction = Direction;

        for (var i = 0; i < dto.Functions.Count; i++)
        {
            orderedFunctions[i].State = FunctionStates[i];
        }
    }
}
