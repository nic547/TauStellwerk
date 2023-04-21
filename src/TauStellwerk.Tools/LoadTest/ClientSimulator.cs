// <copyright file="ClientSimulator.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Immutable;
using TauStellwerk.Client.Services;
using TauStellwerk.Tools.LoadTest.ClientActions;
using TauStellwerk.Util;
using Timer = System.Timers.Timer;

namespace TauStellwerk.Tools.LoadTest;

/// <summary>
/// Simulates a single client that makes it's HTTP Requests.
/// </summary>
public class ClientSimulator
{
    private readonly CounterDictionary _results = new();

    private readonly List<Timer> _timers = new();

    private readonly ImmutableList<ClientActionBase> _actions;

    private ClientSimulator(ImmutableList<ClientActionBase> actions)
    {
        _actions = actions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientSimulator"/> class.
    /// </summary>
    /// <param name="actionTypes">Types that are assignable from ClientActionBase.</param>
    /// <param name="options"><see cref="LoadTestOptions"/>.</param>
    /// <param name="id">Id of the engine to use.</param>
    /// <returns>A <see cref="Task"/> containing the new instance.</returns>
    public static async Task<ClientSimulator> Create(ImmutableList<Type> actionTypes, LoadTestOptions options, int id)
    {
        var random = new Random();
        var settingsService = new LoadGeneratorSettingsService(options, random);
        var httpService = new ConnectionService(settingsService);
        var engineService = new EngineService(httpService);

        await engineService.AcquireEngine(id);

        var instancedActions = new List<ClientActionBase>();

        foreach (var actionType in actionTypes)
        {
            if (Activator.CreateInstance(actionType) is not ClientActionBase action)
            {
                throw new ArgumentException($"{actionType} could not be cast to ClientActionBase");
            }

            await action.Initialize(engineService, options, id, random);
            _ = await action.PerformRequest(); // Perform a first request so that overhead of it doesn't factor into the measurement.
            instancedActions.Add(action);
        }

        return new ClientSimulator(instancedActions.ToImmutableList());
    }

    /// <summary>
    /// Starts the client simulation.
    /// </summary>
    public void Start()
    {
        foreach (var action in _actions)
        {
            var timer = new Timer(action.Interval);
            timer.Elapsed += async (_, _) => { _results.Increment(await action.PerformRequest()); };
            timer.Start();
            _timers.Add(timer);
        }
    }

    /// <summary>
    /// Stops the ClientSimulation.
    /// </summary>
    public void Stop()
    {
        foreach (var timer in _timers)
        {
            timer.Stop();
            timer.Dispose();
        }
    }

    /// <summary>
    /// Get the latency results of this ClientSimulator.
    /// </summary>
    /// <returns>A dictionary, containing the latency in ms as Key and the number of occurrences as Value.</returns>
    public CounterDictionary GetStatistics()
    {
        return _results;
    }
}
