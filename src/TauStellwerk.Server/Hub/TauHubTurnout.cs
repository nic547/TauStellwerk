// <copyright file="TauHubTurnout.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Server.Hub;

public partial class TauHub
{
    public async Task<ResultDto> SetTurnout(int id, State state)
    {
        var turnoutResult = await _turnoutDao.GetTurnoutById(id);

        if (turnoutResult.IsFailed)
        {
           return turnoutResult.ToResult();
        }

        return await _turnoutService.SetState(turnoutResult.Value, state);
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
}