// <copyright file="TauHubEngine.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using FluentResults;
using FluentResults.Extensions;
using TauStellwerk.Base;

namespace TauStellwerk.Server.Hub;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Members are called via SignalR.")]
public partial class TauHub
{
    public async Task<ResultDto<EngineFullDto>> AcquireEngine(int id)
    {
        var sessionResult = _sessionService.TryGetSession(Context.ConnectionId);

        return await sessionResult
            .Bind(_ => _engineDao.GetEngine(id))
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

    public async Task<Result<EngineFullDto>> GetEngine(int id)
    {
        var engine = await _engineDao.GetEngine(id);
        return engine.Bind((engine) => Result.Ok(engine.ToEngineFullDto()));
    }

    public async Task<IList<EngineOverviewDto>> GetEngines(
        int page,
        SortEnginesBy sorting = SortEnginesBy.LastUsed,
        bool sortDescending = true,
        bool showHidden = false)
    {
        var list = await _engineDao.GetEngineList(page, showHidden, sorting, sortDescending);
        return list;
    }

    public async Task<ResultDto<EngineFullDto>> AddOrUpdateEngine(EngineFullDto engine)
    {
        if (engine.Id is not 0)
        {
            return await _sessionService.TryGetSession(Context.ConnectionId)
                .Bind(session => _engineService.CheckIsEngineAcquiredBySession(session, engine.Id))
                .Bind(() => _engineDao.UpdateOrAdd(engine));
        }

        return await _engineDao.UpdateOrAdd(engine);
    }

    public async Task<ResultDto> DeleteEngine(int id)
    {
        var sessionResult = _sessionService.TryGetSession(Context.ConnectionId);

        return await sessionResult
            .Bind(session => _engineService.CheckIsEngineAcquiredBySession(session, id))
            .Bind(() => _engineDao.Delete(id))
            .Bind(() => _engineService.ReleaseEngine(sessionResult.Value, id));
    }
}