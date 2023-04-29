// <copyright file="DccExSerialCommandStation.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO.Ports;
using FluentResults;
using TauStellwerk.Base;
using TauStellwerk.Server.Data.Model;

namespace TauStellwerk.Server.CommandStations;

public class DccExSerialCommandStation : CommandStationBase
{
    private readonly DccExSerialOptions _options;
    private readonly ILogger<CommandStationBase> _logger;
    private readonly SerialPort _serialPort;

    private readonly SemaphoreSlim _writeSemaphore = new(1);

    public DccExSerialCommandStation(DccExSerialOptions options, ILogger<CommandStationBase> logger)
        : base()
    {
        _options = options;
        _logger = logger;
        _logger.LogWarning("This CommandStation is work-in-progress and experimental. Things will break!");

        _serialPort = new SerialPort
        {
            PortName = options.SerialPort ?? throw new FormatException("No SerialPort for DccExSerial was defined."),
            BaudRate = options.BaudRate,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
            Handshake = Handshake.None,
        };

        _serialPort.Open();

        Task.Run(() =>
        {
            while (true)
            {
                var line = _serialPort.ReadLine();

                if (line == "<p0>")
                {
                    OnStatusChange(State.Off);
                }

                if (line == "<p1>")
                {
                    OnStatusChange(State.On);
                }

                if (line.StartsWith("<* MAIN TRACK POWER RESET"))
                {
                    OnStatusChange(State.On);
                }

                if (line.StartsWith("<* MAIN TRACK POWER OVERLOAD"))
                {
                    var timeout = int.Parse(line.Split(' ')[^2][8..]);

                    // Dcc++ EX uses incremental back-off, starting with 20ms and 40ms. Those two are ignored to not create unnecessary noise.
                    if (timeout > 40)
                    {
                        OnStatusChange(State.Off);
                    }
                }
            }
        });
    }

    public override async Task HandleSystemStatus(State state)
    {
        if (_options.UseJoinMode)
        {
            await Send(state == State.On ? "<1 JOIN>" : "<0>");
        }
        else
        {
            await Send(state == State.On ? "<1>" : "<0>");
        }
    }

    public override async Task HandleEngineSpeed(Engine engine, short speed, Direction priorDirection, Direction newDirection)
    {
        await Send($"<t 1 {engine.Address} {speed} {(newDirection == Direction.Forwards ? 1 : 0)}>");
    }

    public override async Task HandleEngineEStop(Engine engine, Direction priorDirection)
    {
        await Send($"<t 1 {engine.Address} -1 {(priorDirection == Direction.Forwards ? 1 : 0)}>");
    }

    public override async Task HandleEngineFunction(Engine engine, byte functionNumber, State state)
    {
        await Send($"<F {engine.Address} {functionNumber} {(state == State.On ? "1" : "0")}>");
    }

    public override async Task CheckState()
    {
        await Send("<s>");
    }

    public override async Task<bool> TryReleaseEngine(Engine engine, EngineState state)
    {
        if (state.Throttle != 0)
        {
            // Forgetting the engine would cause it to stop which might surprise a user.
            return true;
        }

        await Send($"<- {engine.Address}>");

        // Forgetting an engine will send it a e-stop without the direction bit set (backwards)
        // We need to account for this for now
        // Removal depends on https://github.com/DCC-EX/CommandStation-EX/pull/233 getting merged and making it into a release.
        state.Direction = Direction.Backwards;
        return true;
    }

    public override async Task<Result> HandleTurnout(Turnout turnout, State state)
    {
        if (state == State.On)
        {
            await Send($"<a {turnout.Address} 1>");
        }
        else
        {
            await Send($"<a {turnout.Address} 0>");
        }

        return Result.Ok();
    }

    private async Task Send(string message)
    {
        if (!await _writeSemaphore.WaitAsync(2_500))
        {
            return;
        }

        _serialPort.WriteLine(message);
        _writeSemaphore.Release();
    }
}