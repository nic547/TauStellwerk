// <copyright file="ConsoleCommandStation.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TauStellwerk.Base.Model;
using TauStellwerk.Server.Database.Model;

namespace TauStellwerk.Server.CommandStations;

/// <summary>
/// <see cref="CommandStationBase"/> that just writes everything to the console.
/// </summary>
public class ConsoleCommandStation : CommandStationBase
{
    // ReSharper disable once UnusedParameter.Local
    public ConsoleCommandStation(IConfiguration config)
        : base(config)
    {
    }

    public override Task CheckState()
    {
        return Task.CompletedTask;
    }

    public override Task HandleEngineEStop(Engine engine, Direction priorDirection)
    {
        Console.WriteLine($"{engine} has been emergency-stopped");
        return Task.CompletedTask;
    }

    public override Task HandleEngineFunction(Engine engine, byte functionNumber, State state)
    {
        Console.WriteLine($"{engine} function {functionNumber} has been turned {state}");
        return Task.CompletedTask;
    }

    public override Task HandleEngineSpeed(Engine engine, short speed, Direction priorDirection, Direction newDirection)
    {
        if (priorDirection == newDirection)
        {
            Console.WriteLine($"{engine} speed  has been set to {speed}");
        }
        else
        {
            Console.WriteLine($"{engine} speed  has been set to {speed} and is now driving {newDirection}");
        }

        return Task.CompletedTask;
    }

    public override Task HandleSystemStatus(State state)
    {
        Console.WriteLine($"System was {(state == State.On ? "started" : "stopped")}");
        return Task.CompletedTask;
    }

    public override Task<Result> HandleTurnout(Turnout turnout, State state)
    {
        Console.WriteLine($"Turnout at address {turnout.Address} was {(state == State.On ? "turned on" : "turned off")}");
        return Task.FromResult(Result.Ok());
    }
}