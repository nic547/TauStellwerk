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
    private readonly object _turnoutEventLock = new();

    private EventHandler<TurnoutStateChangedEventArgs>? _eventHandler;

    public TurnoutService(IConnectionService connection)
    {
        _connection = connection;
        _ = Init();
    }

    public event EventHandler<TurnoutStateChangedEventArgs>? TurnoutStateChanged
    {
        add
        {
            lock (_turnoutEventLock)
            {
                if (_eventHandler is null)
                {
                    _ = SubscribeToTurnoutEvents();
                }

                _eventHandler += value;
            }
        }

        remove
        {
            lock (_turnoutEventLock)
            {
                _eventHandler -= value;

                if (_eventHandler is null)
                {
                    _ = UnsubscribeFromTurnoutEvents();
                }
            }
        }
    }

    public async Task Init()
    {
        var connection = await _connection.GetHubConnection();
        connection.On<int, State>("HandleTurnoutChange", (address, state) =>
        {
            lock (_turnoutEventLock)
            {
                _eventHandler?.Invoke(this, new TurnoutStateChangedEventArgs(address, state));
            }
        });
    }

    public async Task<Result> ToggleState(Turnout turnout)
    {
        var connection = await _connection.GetHubConnection();
        turnout.State = turnout.State == State.On ? State.Off : State.On;

        if (turnout.Address != 0)
        {
            lock (_turnoutEventLock)
            {
                _eventHandler?.Invoke(this, new TurnoutStateChangedEventArgs(turnout.Address, turnout.State));
            }
        }

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
        throw new NotImplementedException();
    }

    private async Task SubscribeToTurnoutEvents()
    {
        var connection = await _connection.GetHubConnection();
        await connection.InvokeAsync("SubscribeToTurnoutEvents");
    }

    private async Task UnsubscribeFromTurnoutEvents()
    {
        var connection = await _connection.GetHubConnection();
        await connection.InvokeAsync("UnsubscribeFromTurnoutEvents");
    }
}