// <copyright file="ECosConnectionHandler.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PiStellwerk.Util;

namespace PiStellwerk.Commands.ECoS
{
    /// <summary>
    /// Handles the connection to the ECoS, including sending and receiving stuff.
    /// </summary>
    public class ECosConnectionHandler
    {
        private readonly MultiDictionary<string, Action<string>> _events = new();
        private readonly MultiDictionary<string, TaskCompletionSource<string>> _sentCommands = new();

        private readonly TcpClient _client = new();

        private readonly Regex _regex = new("<(?<Type>EVENT|REPLY) (?<Command>.*)>(?<Message>[\\s\\S]*?)<END (?<ErrorCode>\\d*) \\((?<ErrorMessage>.*)\\)>\\r?\\n", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ECosConnectionHandler"/> class.
        /// </summary>
        /// <param name="address">IP of the ECoS.</param>
        /// <param name="port">Port the ECoS listens on.</param>
        public ECosConnectionHandler(IPAddress address, int port)
        {
            _client.Connect(address, port);
            Task.Run(ReceiveData);
        }

        /// <summary>
        /// Send a command, consisting of a string, to the ECoS and return the response.
        /// </summary>
        /// <param name="command"><see cref="string"/> being the command to send.</param>
        /// <returns>Task containing the reply message.</returns>
        public async Task<string> SendCommandAsync(string command)
        {
            await _client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(command));
            var tcs = new TaskCompletionSource<string>();
            _sentCommands.Add(command, tcs);

            return await tcs.Task;
        }

        /// <summary>
        /// Register an event.
        /// </summary>
        /// <param name="command">The command with which to register the command.</param>
        /// <param name="eventName">The name of the event that gets sent.</param>
        /// <param name="action"><see cref="Action"/> that handles the event message.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RegisterEvent(string command, string eventName, Action<string> action)
        {
            await SendCommandAsync(command);
            _events.Add(eventName, action);
        }

        private async void ReceiveData()
        {
            var stream = _client.GetStream();

            var buffer = new byte[1024];

            var receivedBytes = new List<byte>();
            var receivedString = string.Empty;

            while (true)
            {
                var readBytes = await stream.ReadAsync(buffer.AsMemory(0, 1024));
                receivedBytes.AddRange(buffer.SkipLast(1024 - readBytes));
                try
                {
                    receivedString += Encoding.UTF8.GetString(receivedBytes.ToArray());
                    receivedBytes.Clear();
                    Console.WriteLine(receivedString);
                    receivedString = HandleReceivedString(receivedString);
                }
                catch (ArgumentException)
                {
                    // ignored
                }
            }
        }

        private string HandleReceivedString(string input)
        {
            var match = _regex.Match(input);
            var type = match.Groups["Type"].Value;
            var command = match.Groups["Command"].Value;
            var message = match.Groups["Message"].Value;
            _ = int.TryParse(match.Groups["ErrorCode"].Value, out var errorCode);

            if (!match.Success)
            {
                return input;
            }

            switch (type)
            {
                case "EVENT":
                    _events.TryGetAllValues(command, out var actions);

                    if (actions != null)
                    {
                        foreach (var action in actions)
                        {
                            action.Invoke(message);
                        }
                    }

                    break;

                case "REPLY":
                    _sentCommands.TryRemoveFirst(command, out var tcs);
                    if (tcs == null)
                    {
                        Console.WriteLine($"Received reply for {command}, but command was not located. Message:{message}");
                    }
                    else
                    {
                        if (errorCode == 0)
                        {
                            tcs.SetResult(message);
                        }
                        else
                        {
                            // TODO: Handle error responses.
                        }
                    }

                    break;

                default:
                    throw new InvalidDataException($"ECoS Response has unknown type \"{type}\"");
            }

            return input.Remove(match.Index, match.Length);
        }
    }
}
