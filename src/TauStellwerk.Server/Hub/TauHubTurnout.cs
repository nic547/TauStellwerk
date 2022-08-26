// <copyright file="TauHubTurnout.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentResults;
using TauStellwerk.Base;

namespace TauStellwerk.Server.Hub;

public partial class TauHub
{
    private const string TurnoutGroupName = "turnouts";

    public async Task<ResultDto> SetTurnout(int id, State state)
    {
        var turnoutResult = await _turnoutDao.GetTurnoutById(id);

        if (turnoutResult.IsFailed)
        {
           return turnoutResult.ToResult();
        }

        var turnout = turnoutResult.Value;
        var result = await _turnoutService.SetState(turnout, state);

        if (result.IsSuccess && turnout.Address is not 0)
        {
            _ = Clients.OthersInGroup(TurnoutGroupName)
                .SendCoreAsync("HandleTurnoutChange", new[] { (object)turnout.Address, state });
        }

        return result;
    }

    public async Task<IList<TurnoutDto>> GetTurnouts(int page)
    {
        return await _turnoutDao.GetTurnouts(page);
    }

    public async Task<ResultDto> AddOrUpdateTurnout(TurnoutDto dto)
    {
        return await _turnoutDao.AddOrUpdate(dto);
    }

    public async Task<ResultDto> DeleteTurnout(TurnoutDto dto)
    {
        return await _turnoutDao.Delete(dto);
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