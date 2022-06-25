// <copyright file="CommandSystemBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TauStellwerk.Base.Model;
using TauStellwerk.Server.Database;
using TauStellwerk.Server.Database.Model;

namespace TauStellwerk.Server.CommandStation;

/// <summary>
/// Interface for implementing communication with a specific command system.
/// Every implementation needs to be "registered" in <see cref="CommandSystemFactory"/> by hand.
/// </summary>
public abstract class CommandSystemBase
{
    protected CommandSystemBase(IConfiguration configuration)
    {
        Config = configuration;
    }

    public delegate void StatusChangeHandler(State state);

    public event StatusChangeHandler? StatusChanged;

    protected IConfiguration Config { get; }

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
    public virtual Task<bool> TryAcquireEngine(Engine engine)
    {
        return Task.FromResult(true);
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

    protected virtual void OnStatusChange(State state)
    {
        StatusChanged?.Invoke(state);
    }
}