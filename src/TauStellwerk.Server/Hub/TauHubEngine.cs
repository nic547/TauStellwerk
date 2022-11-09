// <copyright file="TauHubEngine.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using FluentResults;
using FluentResults.Extensions;
using Microsoft.AspNetCore.Mvc;
using TauStellwerk.Base;
using TauStellwerk.Server.Dao;

namespace TauStellwerk.Server.Hub;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Members are called via SignalR.")]
public partial class TauHub
{
    public async Task<ResultDto<EngineFullDto>> AcquireEngine([FromServices]EngineDao engineDao, int id)
    {
        var sessionResult = _sessionService.TryGetSession(Context.ConnectionId);

        return await sessionResult
            .Bind(_ => engineDao.GetEngine(id))
            .Bind(engine => _engineService.AcquireEngine(sessionResult.Value, engine));
    }

    public async Task<ResultDto> ReleaseEngine(int id)
    {
        return await _sessionService.TryGetSession(Context.ConnectionId)
            .Bind(session => _engineService.ReleaseEngine(session, id));
    }

    public async Task<ResultDto> SetEngineSpeed(int id, int speed, Direction? direction)
    {
        return await _sessionService.TryGetSession(Context.ConnectionId)
            .Bind(session => _engineService.SetEngineSpeed(session, id, speed, direction));
    }

    public async Task<ResultDto> SetEngineEStop(int id)
    {
        return await _sessionService.TryGetSession(Context.ConnectionId)
            .Bind(session => _engineService.SetEngineEStop(session, id));
    }

    public async Task<ResultDto> SetEngineFunction(int id, int number, State state)
    {
        return await _sessionService.TryGetSession(Context.ConnectionId)
            .Bind(session => _engineService.SetEngineFunction(session, id, number, state));
    }

    public async Task<Result<EngineFullDto>> GetEngine([FromServices] EngineDao engineDao, int id)
    {
        var engine = await engineDao.GetEngine(id);
        return engine.Bind((engine) => Result.Ok(engine.ToEngineFullDto()));
    }

    public async Task<IList<EngineOverviewDto>> GetEngines(
        [FromServices] EngineDao engineDao,
        int page,
        int sorting,
        bool sortDescending = true,
        bool showHidden = false)
    {
        // With the upgrade to .NET 7.0 SignalR doesn't seem to like the SortEnginesBy enum, so instead we explicitly take an int and just cast that.
        var list = await engineDao.GetEngineList(page, showHidden, (SortEnginesBy)sorting, sortDescending);
        return list;
    }

    public async Task<ResultDto<EngineFullDto>> AddOrUpdateEngine([FromServices] EngineDao engineDao, EngineFullDto engine)
    {
        if (engine.Id is not 0)
        {
            return await _sessionService.TryGetSession(Context.ConnectionId)
                .Bind(session => _engineService.CheckIsEngineAcquiredBySession(session, engine.Id))
                .Bind(() => engineDao.UpdateOrAdd(engine));
        }

        return await engineDao.UpdateOrAdd(engine);
    }

    public async Task<ResultDto> DeleteEngine([FromServices] EngineDao engineDao, int id)
    {
        var sessionResult = _sessionService.TryGetSession(Context.ConnectionId);

        return await sessionResult
            .Bind(session => _engineService.CheckIsEngineAcquiredBySession(session, id))
            .Bind(() => engineDao.Delete(id))
            .Bind(() => _engineService.ReleaseEngine(sessionResult.Value, id));
    }
}