// <copyright file="ECosConnectionHandler.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;
using TauStellwerk.Util;

namespace TauStellwerk.Server.CommandStations;

/// <summary>
/// Handles the connection to the ECoS, including sending and receiving stuff.
/// </summary>
public class ECosConnectionHandler
{
    private const int BufferSize = 8192; // Assumption: An ECoS doesn't send lines longer than this size.

    private readonly ILogger<CommandStationBase> _logger;

    private readonly MultiDictionary<string, Action<ECoSMessage>> _events = new();
    private readonly MultiDictionary<string, TaskCompletionSource<ECoSMessage>> _sentCommands = new();

    private readonly TcpClient _client = new();

    private readonly SemaphoreSlim _writeSemaphore = new(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="ECosConnectionHandler"/> class.
    /// </summary>
    /// <param name="address">IP of the ECoS.</param>
    /// <param name="port">Port the ECoS listens on.</param>
    /// <param name="logger">The ILogger to use.</param>
    public ECosConnectionHandler(IPAddress address, int port, ILogger<CommandStationBase> logger)
    {
        _logger = logger;
        if (TryOpenConnection(address, port).IsFailed)
        {
            throw new Exception("Cannot establish connection to ECoS");
        }

        Task.Run(ReceiveData);
    }

    /// <summary>
    /// Send a command, consisting of a string, to the ECoS and return the response.
    /// </summary>
    /// <param name="command"><see cref="string"/> being the command to send.</param>
    /// <returns>Task containing the reply message.</returns>
    public async Task<Result<ECoSMessage>> SendCommandAsync(string command)
    {
        if (await _writeSemaphore.WaitAsync(2500))
        {
            await _client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(command));

            _writeSemaphore.Release();

            var tcs = new TaskCompletionSource<ECoSMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
            _sentCommands.Add(command, tcs);
            return Result.Ok(await tcs.Task);
        }

        _logger.LogCritical("ECoSCommandSystem could not send message - ECoS might be overloaded.");
        return Result.Fail("Couldn't acquire lock - ECoS might be overloaded");
    }

    /// <summary>
    /// Register an event.
    /// </summary>
    /// <param name="command">The command with which to register the command.</param>
    /// <param name="eventName">The name of the event that gets sent.</param>
    /// <param name="action"><see cref="Action"/> that handles the event message.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task RegisterEvent(string command, string eventName, Action<ECoSMessage> action)
    {
        await SendCommandAsync(command);
        _events.Add(eventName, action);
    }

    private Result TryOpenConnection(IPAddress address, int port)
    {
        var tries = 0;
        while (tries < 10)
        {
            try
            {
                _client.Connect(address, port);
                return Result.Ok();
            }
            catch (Exception)
            {
                tries++;
                Thread.Sleep(2000);
            }
        }

        _logger.LogCritical("Failed to establish connection to ECoS at {address}:{port}", address, port);
        return Result.Fail("Failed to establish connection to ECoS");
    }

    private async void ReceiveData()
    {
        var stream = _client.GetStream();

        StreamReader reader = new(stream, Encoding.UTF8, false, BufferSize, false);

        List<string> lines = new();

        while (true)
        {
            var line = await reader.ReadLineAsync();
            lines.Add(line ?? throw new InvalidOperationException("Connection to ECoS seems to be lost."));

            if (line.StartsWith("<END"))
            {
                HandleReceivedMessages(new ECoSMessage(lines));
                lines.Clear();
            }
        }
    }

    private void HandleReceivedMessages(ECoSMessage message)
    {
        switch (message.Type)
        {
            case "EVENT":
                _events.TryGetAllValues(message.Command, out var actions);
                if (actions != null)
                {
                    foreach (var action in actions)
                    {
                        action.Invoke(message);
                    }
                }

                break;

            case "REPLY":
                _sentCommands.TryRemoveFirst(message.Command, out var tcs);
                if (tcs != null)
                {
                    tcs.SetResult(message);
                }
                else
                {
                    _logger.LogError("Received reply for {command}, but command was not located. Message:{message}", message.Command, message);
                }

                break;

            default:
                throw new InvalidDataException($"ECoS Response has unknown type \"{message.Type}\"");
        }
    }
}