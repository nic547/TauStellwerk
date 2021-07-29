﻿// <copyright file="DccExSerialSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO.Ports;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PiStellwerk.Database.Model;
using PiStellwerk.Util;

namespace PiStellwerk.Commands
{
    public class DccExSerialSystem : CommandSystemBase
    {
        private SerialPort _serialPort;

        public DccExSerialSystem(IConfiguration configuration)
            : base(configuration)
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = Config["CommandSystem:SerialPort"];
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
                    ConsoleService.PrintMessage(_serialPort.ReadLine());
                }
            });
        }

        public override Task HandleSystemStatus(bool shouldBeRunning)
        {
            _serialPort.WriteLine(shouldBeRunning ? "<1>" : "<0>");
            return Task.CompletedTask;
        }

        public override Task HandleEngineSpeed(Engine engine, short speed, bool hasBeenDrivingForwards, bool shouldBeDrivingForwards)
        {
            throw new NotImplementedException();
        }

        public override Task HandleEngineEStop(Engine engine)
        {
            throw new NotImplementedException();
        }

        public override Task HandleEngineFunction(Engine engine, byte functionNumber, bool on)
        {
            throw new NotImplementedException();
        }
    }
}