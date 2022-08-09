// <copyright file="TurnoutService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Immutable;
using FluentResults;
using Microsoft.AspNetCore.SignalR.Client;
using TauStellwerk.Base;
using TauStellwerk.Client.Model;

namespace TauStellwerk.Client.Services;

public class TurnoutService : ITurnoutService
{
    private readonly IConnectionService _connection;

    public TurnoutService(IConnectionService connection)
    {
        _connection = connection;
    }

    public async Task<Result> ToggleState(Turnout turnout)
    {
        var connection = await _connection.GetHubConnection();
        turnout.State = turnout.State == State.On ? State.Off : State.On;
        await connection.InvokeAsync("SetTurnout", turnout.Id, turnout.State);
        return Result.Ok();
    }

    public async Task<IImmutableList<Turnout>> GetList(int page = 0)
    {
        var connection = await _connection.GetHubConnection();
        var dtos = await connection.InvokeAsync<IList<TurnoutDto>>("GetTurnouts", page);
        return dtos.Select(dto => new Turnout(dto)).ToImmutableList();
    }

    public async Task<ResultDto> AddOrUpdate(Turnout turnout)
    {
        var connection = await _connection.GetHubConnection();
        return await connection.InvokeAsync<ResultDto>("AddOrUpdateTurnout", turnout.ToDto());
    }

    public Task<ResultDto> Delete(Turnout turnout)
    {
        throw new System.NotImplementedException();
    }
}