// <copyright file="TauHubEngine.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentResults;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Hub;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Members are called via SignalR.")]
public partial class TauHub
{
    public async Task<ResultDto<EngineFullDto>> AcquireEngine(int id)
    {
        var session = _sessionService.TryGetSession(Context.ConnectionId);

        if (session == null)
        {
            return Result.Fail("Session not found");
        }

        var engineResult = await _engineRepo.GetEngine(id);
        if (!engineResult.IsSuccess)
        {
            return Result.Fail("Engine not found");
        }

        await _engineRepo.UpdateLastUsed(id);
        var acquireResult = await _engineService.AcquireEngine(session, engineResult.Value);

        return acquireResult;
    }

    public async Task<Result> ReleaseEngine(int id)
    {
        var session = _sessionService.TryGetSession(Context.ConnectionId);

        if (session == null)
        {
            return Result.Fail("Session not found");
        }

        return await _engineService.ReleaseEngine(session, id);
    }

    public async Task<Result> SetEngineSpeed(int id, int speed, Direction? direction)
    {
        var session = _sessionService.TryGetSession(Context.ConnectionId);

        if (session == null)
        {
            return Result.Fail("Session not found");
        }

        return await _engineService.SetEngineSpeed(session, id, speed, direction);
    }

    public async Task<Result> SetEngineEStop(int id)
    {
        var session = _sessionService.TryGetSession(Context.ConnectionId);

        if (session == null)
        {
            return Result.Fail("Session not found");
        }

        return await _engineService.SetEngineEStop(session, id);
    }

    public async Task<Result> SetEngineFunction(int id, int number, State state)
    {
        var session = _sessionService.TryGetSession(Context.ConnectionId);

        if (session == null)
        {
            return Result.Fail("Session not found");
        }

        return await _engineService.SetEngineFunction(session, id, number, state);
    }

    public async Task<Result<EngineFullDto>> GetEngine(int id)
    {
        return await _engineRepo.GetEngineFullDto(id);
    }

    public async Task<IList<EngineDto>> GetEngines(int page, SortEnginesBy sorting = SortEnginesBy.LastUsed, bool sortDescending = true, bool showHidden = false)
    {
        var list = await _engineRepo.GetEngineList(page, showHidden, sorting, sortDescending);
        return list;
    }

    public async Task<ResultDto<EngineFullDto>> AddOrUpdateEngine(EngineFullDto engine)
    {
        return await _engineRepo.UpdateOrAdd(engine);
    }

    public async Task DeleteEngine(int id)
    {
        await _engineRepo.Delete(id);
    }
}
