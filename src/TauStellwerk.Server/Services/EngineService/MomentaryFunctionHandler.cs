// <copyright file="MomentaryFunctionHandler.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TauStellwerk.Base.Model;
using TauStellwerk.Server.CommandStation;
using TauStellwerk.Server.Database.Model;
using Timer = System.Timers.Timer;

namespace TauStellwerk.Server.Services.EngineService;

public class MomentaryFunctionHandler
{
    private readonly CommandSystemBase _commandSystem;
    private readonly int _timeDecrement;

    private readonly List<Entry> _activeFunctions = new();

    private readonly SemaphoreSlim _listLock = new(1);
    private readonly Timer _timer;

    public MomentaryFunctionHandler(CommandSystemBase commandSystem, int timeBetweenRuns)
    {
        _commandSystem = commandSystem;
        var timeBetweenRuns1 = timeBetweenRuns;

        // Chosen so functions are turned off slighty early - doesn't seem to have an effect on the function, but it's certainly "ready" for the next activation.
        _timeDecrement = Convert.ToInt32(timeBetweenRuns1 * 1.2);

        _timer = new Timer(timeBetweenRuns1);
        _timer.Elapsed += async (_, _) =>
        {
            await HandleMomentaryFunctions();
        };

        _timer.Start();
    }

    public void Add(Engine engine, DccFunction function)
    {
        _listLock.Wait();
        _activeFunctions.Add(new Entry(engine, function.Duration, function.Number));
        _listLock.Release();
        _timer.Start();
    }

    private async Task HandleMomentaryFunctions()
    {
        _listLock.Wait();

        for (int i = 0; i < _activeFunctions.Count; i++)
        {
            var entry = _activeFunctions[i];
            if (entry.RemainingDuration < 0)
            {
                await _commandSystem.HandleEngineFunction(entry.Engine, entry.FunctionNumber, State.Off);
                _activeFunctions.RemoveAt(i);
            }
            else
            {
                entry.RemainingDuration -= _timeDecrement;
            }
        }

        _listLock.Release();

        if (!_activeFunctions.Any())
        {
            _timer.Stop();
        }
    }

    private class Entry
    {
        public Entry(Engine engine, int duration, byte functionNumber)
        {
            Engine = engine;
            RemainingDuration = duration;
            FunctionNumber = functionNumber;
        }

        public Engine Engine { get; }

        public int RemainingDuration { get; set; }

        public byte FunctionNumber { get; }
    }
}
