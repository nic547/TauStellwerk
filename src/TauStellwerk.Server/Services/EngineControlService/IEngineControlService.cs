// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.


using FluentResults;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Server.Services.EngineControlService;

public interface IEngineControlService
{
    Task<Result<EngineFullDto>> AcquireEngine(Session session, Engine engine);

    Task<Result> ReleaseEngine(Session session, int engineId);

    Task<Result> SetEngineSpeed(Session session, int engineId, int speed, Direction? newDirection);

    Task<Result> SetEngineEStop(Session session, int engineId);

    Task<Result> SetEngineFunction(Session session, int engineId, int functionNumber, State state);

    Result CheckIsEngineAcquiredBySession(Session session, int engineId);
}
