// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using FluentResults;
using FluentResults.Extensions;
using Microsoft.AspNetCore.Mvc;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;
using TauStellwerk.CommandStations;
using TauStellwerk.Data.Dao;

namespace TauStellwerk.Server.Hub;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Members are called via SignalR.")]
public partial class TauHub
{
    public async Task<ResultDto<EngineFullDto>> AcquireEngine([FromServices] EngineDao engineDao, int id)
    {
        var sessionResult = _sessionService.TryGetSession(Context.ConnectionId);

        return await sessionResult
            .Bind(_ => engineDao.GetEngine(id))
            .Bind(engine => _engineControlService.AcquireEngine(sessionResult.Value, engine));
    }

    public async Task<ResultDto> ReleaseEngine(int id)
    {
        return await _sessionService.TryGetSession(Context.ConnectionId)
            .Bind(session => _engineControlService.ReleaseEngine(session, id));
    }

    public async Task<ResultDto> SetEngineSpeed(int id, int speed, Direction? direction)
    {
        return await _sessionService.TryGetSession(Context.ConnectionId)
            .Bind(session => _engineControlService.SetEngineSpeed(session, id, speed, direction));
    }

    public async Task<ResultDto> SetEngineEStop(int id)
    {
        return await _sessionService.TryGetSession(Context.ConnectionId)
            .Bind(session => _engineControlService.SetEngineEStop(session, id));
    }

    public async Task<ResultDto> SetEngineFunction(int id, int number, State state)
    {
        return await _sessionService.TryGetSession(Context.ConnectionId)
            .Bind(session => _engineControlService.SetEngineFunction(session, id, number, state));
    }

    public async Task<Result<EngineFullDto>> GetEngine([FromServices] EngineDao engineDao, int id)
    {
        var engine = await engineDao.GetEngine(id);
        return engine.Bind((engine) => Result.Ok(engine.ToEngineFullDto()));
    }

    public async Task<IList<EngineOverviewDto>> GetEngines(
        [FromServices] EngineDao engineDao,
        string searchTerm,
        int page,
        SortEnginesBy sorting,
        bool sortDescending = true,
        bool showHidden = false)
    {
        var list = await engineDao.GetEngineList(searchTerm, page, showHidden, sorting, sortDescending);
        return list;
    }

    public async Task<ResultDto<EngineFullDto>> AddOrUpdateEngine([FromServices] EngineDao engineDao, EngineFullDto engine)
    {
        if (engine.Id is not 0)
        {
            return await _sessionService.TryGetSession(Context.ConnectionId)
                .Bind(session => _engineControlService.CheckIsEngineAcquiredBySession(session, engine.Id))
                .Bind(() => engineDao.UpdateOrAdd(engine));
        }

        return await engineDao.UpdateOrAdd(engine);
    }

    public async Task<ResultDto> DeleteEngine([FromServices] EngineDao engineDao, int id)
    {
        var sessionResult = _sessionService.TryGetSession(Context.ConnectionId);

        return await sessionResult
            .Bind(session => _engineControlService.CheckIsEngineAcquiredBySession(session, id))
            .Bind(() => engineDao.Delete(id))
            .Bind(() => _engineControlService.ReleaseEngine(sessionResult.Value, id));
    }

    public async Task<ResultDto<int>> ReadDccAddress([FromServices] CommandStationBase commandStation)
    {
        return await commandStation.ReadDccAddress();
    }

    public async Task<ResultDto> WriteDccAddress([FromServices] CommandStationBase commandStation, int address)
    {
        return await commandStation.WriteDccAddress(address);
    }
}
