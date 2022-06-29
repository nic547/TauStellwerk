// <copyright file="TauHubEngine.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentResults;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Server.Hub;

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

    public async Task<ResultDto> ReleaseEngine(int id)
    {
        var session = _sessionService.TryGetSession(Context.ConnectionId);

        if (session == null)
        {
            return Result.Fail("Session not found");
        }

        return await _engineService.ReleaseEngine(session, id);
    }

    public async Task<ResultDto> SetEngineSpeed(int id, int speed, Direction? direction)
    {
        var session = _sessionService.TryGetSession(Context.ConnectionId);

        if (session == null)
        {
            return Result.Fail("Session not found");
        }

        return await _engineService.SetEngineSpeed(session, id, speed, direction);
    }

    public async Task<ResultDto> SetEngineEStop(int id)
    {
        var session = _sessionService.TryGetSession(Context.ConnectionId);

        if (session == null)
        {
            return Result.Fail("Session not found");
        }

        return await _engineService.SetEngineEStop(session, id);
    }

    public async Task<ResultDto> SetEngineFunction(int id, int number, State state)
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

    public async Task<IList<EngineOverviewDto>> GetEngines(
        int page,
        SortEnginesBy sorting = SortEnginesBy.LastUsed,
        bool sortDescending = true,
        bool showHidden = false)
    {
        var list = await _engineRepo.GetEngineList(page, showHidden, sorting, sortDescending);
        return list;
    }

    public async Task<ResultDto<EngineFullDto>> AddOrUpdateEngine(EngineFullDto engine)
    {
        if (engine.Id is not 0)
        {
            var session = _sessionService.TryGetSession(Context.ConnectionId);
            if (session is null)
            {
                return Result.Fail("No session found!");
            }

            var result = _engineService.IsEngineAcquiredBySession(session, engine.Id);
            if (result.IsFailed)
            {
                return result;
            }
        }

        return await _engineRepo.UpdateOrAdd(engine);
    }

    public async Task<ResultDto> DeleteEngine(int id)
    {
        var session = _sessionService.TryGetSession(Context.ConnectionId);
        if (session is null)
        {
            return Result.Fail("No session found!");
        }

        var result = _engineService.IsEngineAcquiredBySession(session, id);
        if (result.IsFailed)
        {
            return result;
        }

        var deleteResult = await _engineRepo.Delete(id);
        if (deleteResult.IsFailed)
        {
            return deleteResult;
        }

        await _engineService.ReleaseEngine(session, id);
        return Result.Ok();
    }
}