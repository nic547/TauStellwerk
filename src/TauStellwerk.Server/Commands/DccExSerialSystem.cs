// <copyright file="DccExSerialSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TauStellwerk.Base.Model;
using TauStellwerk.Database.Model;

namespace TauStellwerk.Commands;

public class DccExSerialSystem : CommandSystemBase
{
    private readonly ILogger<CommandSystemBase> _logger;
    private readonly SerialPort _serialPort;

    private readonly SemaphoreSlim _writeSemaphore = new(1);

    private State? _currentState;

    public DccExSerialSystem(IConfiguration configuration, ILogger<CommandSystemBase> logger)
        : base(configuration)
    {
        _logger = logger;
        logger.LogWarning("This CommandSystem is work-in-progress and experimental. Things will break!");

        _serialPort = new SerialPort
        {
            PortName = Config["CommandSystem:SerialPort"],
        };
        _ = int.TryParse(Config["CommandSystem:BaudRate"] ?? "115200", out var baudRate);
        _serialPort.BaudRate = baudRate;
        _serialPort.Parity = Parity.None;
        _serialPort.DataBits = 8;
        _serialPort.StopBits = StopBits.One;
        _serialPort.Handshake = Handshake.None;

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
        _currentState = state;
        await Send(state == State.On ? "<1>" : "<0>");
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

    protected override void OnStatusChange(State state)
    {
        // Only actually notify of state changes when they aren't known yet.
        if (state != _currentState)
        {
            _currentState = state;
            base.OnStatusChange(state);
        }
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