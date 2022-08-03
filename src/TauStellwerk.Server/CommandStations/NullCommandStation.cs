// <copyright file="NullCommandStation.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentResults;
using TauStellwerk.Base.Model;
using TauStellwerk.Server.Database.Model;

namespace TauStellwerk.Server.CommandStations;

/// <summary>
/// Implements a <see cref="CommandStationBase"/> that does nothing.
/// </summary>
public class NullCommandStation : CommandStationBase
{
    public NullCommandStation(IConfiguration config)
        : base(config)
    {
    }

    /// <inheritdoc/>
    public override Task HandleEngineEStop(Engine engine, Direction priorDirection)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task HandleEngineFunction(Engine engine, byte functionNumber, State state)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task HandleSystemStatus(State state)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override Task HandleEngineSpeed(Engine engine, short speed, Direction priorDirection, Direction newDirection)
    {
        return Task.CompletedTask;
    }

    public override Task CheckState()
    {
        return Task.CompletedTask;
    }

    public override Task<Result> HandleTurnout(Turnout turnout, State state)
    {
        return Task.FromResult(Result.Ok());
    }
}