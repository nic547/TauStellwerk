// <copyright file="TauHubTurnout.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using TauStellwerk.Base;
using TauStellwerk.Data.Dao;

namespace TauStellwerk.Server.Hub;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Members are called via SignalR.")]
public partial class TauHub
{
    private const string TurnoutGroupName = "turnouts";

    public async Task<ResultDto> SetTurnout([FromServices] ITurnoutDao turnoutDao, int id, State state)
    {
        var turnoutResult = await turnoutDao.GetTurnoutById(id);

        if (turnoutResult.IsFailed)
        {
            return turnoutResult.ToResult();
        }

        var turnout = turnoutResult.Value;
        var result = await _turnoutControlService.SetState(turnout, state);

        if (result.IsSuccess && turnout.Address is not 0)
        {
            _ = Clients.OthersInGroup(TurnoutGroupName)
                .SendCoreAsync("HandleTurnoutChange", new[] { (object)turnout.Address, state });
        }

        return result;
    }

    public async Task<IList<TurnoutDto>> GetTurnouts([FromServices] ITurnoutDao turnoutDao, int page)
    {
        var turnouts = await turnoutDao.GetTurnouts(page);
        return _turnoutControlService.GetTurnoutsWithState(turnouts).Select(t => t.ToDto()).ToList();
    }

    public async Task<ResultDto> AddOrUpdateTurnout([FromServices] ITurnoutDao turnoutDao, TurnoutDto dto)
    {
        return await turnoutDao.AddOrUpdate(dto);
    }

    public async Task<ResultDto> DeleteTurnout([FromServices] ITurnoutDao turnoutDao, TurnoutDto dto)
    {
        return await turnoutDao.Delete(dto);
    }

    public async Task<ResultDto> SubscribeToTurnoutEvents()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, TurnoutGroupName);
        _logger.LogDebug("Client {client} subscribed to TurnoutEvents", _sessionService.TryGetSession(Context.ConnectionId));
        return Result.Ok();
    }

    public async Task<ResultDto> UnsubscribeFromTurnoutEvents()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, TurnoutGroupName);
        _logger.LogDebug("Client {client} unsubscribed from TurnoutEvents", _sessionService.TryGetSession(Context.ConnectionId));
        return Result.Ok();
    }
}