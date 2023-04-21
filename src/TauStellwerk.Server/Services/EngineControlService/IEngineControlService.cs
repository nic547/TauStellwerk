// <copyright file="IEngineControlService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using FluentResults;
using TauStellwerk.Base;
using TauStellwerk.Server.Data.Model;

namespace TauStellwerk.Server.Services;

public interface IEngineControlService
{
    Task<Result<EngineFullDto>> AcquireEngine(Session session, Engine engine);

    Task<Result> ReleaseEngine(Session session, int engineId);

    Task<Result> SetEngineSpeed(Session session, int engineId, int speed, Direction? newDirection);

    Task<Result> SetEngineEStop(Session session, int engineId);

    Task<Result> SetEngineFunction(Session session, int engineId, int functionNumber, State state);

    Result CheckIsEngineAcquiredBySession(Session session, int engineId);
}