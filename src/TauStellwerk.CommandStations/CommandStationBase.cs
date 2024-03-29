﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.


using FluentResults;
using TauStellwerk.Base.Model;
using TauStellwerk.Data;
using TauStellwerk.Data.Model;

namespace TauStellwerk.CommandStations;

/// <summary>
/// Interface for implementing communication with a specific command system.
/// </summary>
public abstract class CommandStationBase
{
    public delegate void StatusChangeHandler(State state);

    public event StatusChangeHandler? StatusChanged;

    public abstract Task HandleSystemStatus(State desiredState);

    public abstract Task HandleEngineSpeed(Engine engine, short speed, Direction priorDirection, Direction newDirection);

    public abstract Task HandleEngineEStop(Engine engine, Direction priorDirection);

    public abstract Task HandleEngineFunction(Engine engine, byte functionNumber, State state);

    /// <summary>
    /// Load engines from the command system. Will do nothing if the system doesn't know about engines.
    /// </summary>
    /// <param name="context"><see cref="StwDbContext"/> to compare/insert engines.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public virtual Task LoadEnginesFromSystem(StwDbContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// "Acquire" an engine - Some command system may require explicitly taking control of a engine before sending commands.
    /// <see cref="TryReleaseEngine"/> to "release" an engine again.
    /// </summary>
    /// <param name="engine">The engine to acquire.</param>
    /// <returns>A bool indicating whether the operation was successful.</returns>
    public virtual Task<Result> TryAcquireEngine(Engine engine)
    {
        return Task.FromResult(Result.Ok());
    }

    /// <summary>
    /// "Release" an engine.
    /// <see cref="TryAcquireEngine"/> to "acquire" an engine again.
    /// </summary>
    /// <param name="engine">The engine to release.</param>
    /// <param name="state"><see cref="EngineState"/>representing the current state of the engine.</param>
    /// <returns>A bool indicating whether the operation was successful.</returns>
    public virtual Task<bool> TryReleaseEngine(Engine engine, EngineState state)
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Ensure that the initial command system state is loaded and trigger <see cref="OnStatusChange"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public abstract Task CheckState();

    public virtual Task<Result> HandleTurnout(Turnout turnout, State state)
    {
        return Task.FromResult(Result.Fail("CommandSystem doesn't support Turnouts"));
    }

    public virtual Task<Result<int>> ReadDccAddress()
    {
        return Task.FromResult(Result.Fail<int>("Programming addresses hasn't been implemented for this command station"));
    }

    public virtual Task<Result> WriteDccAddress(int address)
    {
        return Task.FromResult(Result.Fail("Programming addresses hasn't been implemented for this command station"));
    }

    protected virtual void OnStatusChange(State state)
    {
        StatusChanged?.Invoke(state);
    }
}
