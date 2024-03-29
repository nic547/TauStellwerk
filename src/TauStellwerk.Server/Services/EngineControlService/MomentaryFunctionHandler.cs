﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Base.Model;
using TauStellwerk.CommandStations;
using TauStellwerk.Data.Model;
using TauStellwerk.Util.DateTimeProvider;
using TauStellwerk.Util.Timer;
using ITimer = TauStellwerk.Util.Timer.ITimer;

namespace TauStellwerk.Server.Services.EngineControlService;

public class MomentaryFunctionHandler
{
    private readonly CommandStationBase _commandStation;
    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly List<ActiveFunction> _activeFunctions = [];

    private readonly SemaphoreSlim _listLock = new(1);
    private readonly ITimer _timer;

    public MomentaryFunctionHandler(CommandStationBase commandStation, int timeBetweenRuns, ITimer? timer = null, IDateTimeProvider? nowProvider = null)
    {
        _commandStation = commandStation;

        _dateTimeProvider = nowProvider ?? new DateTimeProvider();
        _timer = timer ?? new TimerWrapper(timeBetweenRuns);
        _timer.Elapsed += async (_, signalTime) =>
        {
            await HandleMomentaryFunctions(signalTime.ToUniversalTime());
        };
    }

    public async Task Add(Engine engine, DccFunction function)
    {
        var now = _dateTimeProvider.GetUtcNow();
        var expiry = now.AddMilliseconds(function.Duration * 0.8);

        await _listLock.WaitAsync();
        _activeFunctions.Add(new ActiveFunction(engine, function.Number, expiry));

        _listLock.Release();
        _timer.Start(); // Starting a running timer has no effect => starting them multiple times is not a problem.
    }

    private async Task HandleMomentaryFunctions(DateTime signalTime)
    {
        await _listLock.WaitAsync();

        for (var i = _activeFunctions.Count - 1; i >= 0; i--)
        {
            var activeFunction = _activeFunctions[i];
            if (activeFunction.Expiry <= signalTime)
            {
                await _commandStation.HandleEngineFunction(activeFunction.Engine, activeFunction.FunctionNumber, State.Off);
                _activeFunctions.RemoveAt(i);
            }
        }

        if (_activeFunctions.Count == 0)
        {
            _timer.Stop();
        }

        _listLock.Release();
    }

    private record ActiveFunction(Engine Engine, byte FunctionNumber, DateTime Expiry);
}
