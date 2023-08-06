// <copyright file="TurnoutControlService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections;
using FluentResults;
using TauStellwerk.Base;
using TauStellwerk.CommandStations;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Server.Services;

public class TurnoutControlService
{
    private readonly CommandStationBase _cs;
    private readonly BitArray _turnoutStates = new(2048);

    public TurnoutControlService(CommandStationBase cs)
    {
        _cs = cs;
    }

    public async Task<Result> SetState(Turnout turnout, State state)
    {
        await _cs.HandleTurnout(turnout, state);
        _turnoutStates[turnout.Address] = state == State.On;
        return Result.Ok();
    }

    public IReadOnlyList<Turnout> GetTurnoutsWithState(IReadOnlyList<Turnout> turnouts)
    {
        foreach (var turnout in turnouts)
        {
            turnout.State = _turnoutStates[turnout.Address] ? State.On : State.Off;
        }

        return turnouts;
    }
}