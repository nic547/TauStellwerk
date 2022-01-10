// <copyright file="DccExSerialSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO.Ports;
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
                Console.WriteLine(_serialPort.ReadLine());
            }
        });
    }

    public override Task HandleSystemStatus(State state)
    {
        _serialPort.WriteLine(state == State.On ? "<1>" : "<0>");
        return Task.CompletedTask;
    }

    public override Task HandleEngineSpeed(Engine engine, short speed, Direction priorDirection, Direction newDirection)
    {
        _serialPort.WriteLine($"<t 1 {engine.Address} {speed} {(newDirection == Direction.Forwards ? 1 : 0)}>");
        return Task.CompletedTask;
    }

    public override Task HandleEngineEStop(Engine engine, Direction priorDirection)
    {
        _serialPort.WriteLine($"<t 1 {engine.Address} -1 {(priorDirection == Direction.Forwards ? 1 : 0)}>");
        return Task.CompletedTask;
    }

    public override Task HandleEngineFunction(Engine engine, byte functionNumber, State state)
    {
        _serialPort.WriteLine($"<F {engine.Address} {functionNumber} {(state == State.On ? "1" : "0")}>");
        return Task.CompletedTask;
    }

    public override Task CheckState()
    {
        // TODO #130
        throw new System.NotImplementedException();
    }
}