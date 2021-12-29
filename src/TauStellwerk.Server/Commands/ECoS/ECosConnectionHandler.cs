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
using System.Threading.Tasks;
using TauStellwerk.Util;

namespace TauStellwerk.Commands.ECoS;

/// <summary>
/// Handles the connection to the ECoS, including sending and receiving stuff.
/// </summary>
public class ECosConnectionHandler
{
    private const int BufferSize = 8192; // Assumption: An ECoS doesn't send lines longer than this size.

    private const byte CrChar = 13;
    private const byte LfChar = 10;

    private readonly MultiDictionary<string, Action<ECoSMessage>> _events = new();
    private readonly MultiDictionary<string, TaskCompletionSource<ECoSMessage>> _sentCommands = new();

    private readonly TcpClient _client = new();

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
    public async Task<ECoSMessage> SendCommandAsync(string command)
    {
        await _client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(command));
        var tcs = new TaskCompletionSource<ECoSMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
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
    public async Task RegisterEvent(string command, string eventName, Action<ECoSMessage> action)
    {
        await SendCommandAsync(command);
        _events.Add(eventName, action);
    }

    private async void ReceiveData()
    {
        var stream = _client.GetStream();

        var buffer = new byte[BufferSize];
        var bufferStartPosition = 0;

        List<string> lines = new();

        while (true)
        {
            var readBytes = await stream.ReadAsync(buffer, bufferStartPosition, BufferSize - bufferStartPosition);
            var totalLength = bufferStartPosition + readBytes;

            var (newLines, newLength) = FindLines(buffer, totalLength);
            lines.AddRange(newLines);
            totalLength = newLength;

            lines = HandleReceivedLines(lines);
        }
    }

    private (List<string> Lines, int NewLenght) FindLines(byte[] bytes, int contentLenght)
    {
        List<string> lines = new();
        var indices = FindEndOfLineBytesIndices(bytes, contentLenght);
        var lastLineEndIndex = -1;
        foreach (var index in indices)
        {
            var startIndex = lastLineEndIndex + 1;
            var length = index - startIndex + 1;

            lines.Add(Encoding.UTF8.GetString(bytes, startIndex, length));

            lastLineEndIndex = index;
        }

        var remainingContentStart = lastLineEndIndex + 1;
        var newContentLenght = contentLenght - remainingContentStart;

        MoveContentToArrayStart(ref bytes, remainingContentStart, newContentLenght);

        return (lines, newContentLenght);
    }

    private List<int> FindEndOfLineBytesIndices(byte[] bytes, int contentLenght)
    {
        List<int> occurrences = new();
        for (var i = 0; i <= contentLenght - 1; i++)
        {
            if (bytes[i] == CrChar && bytes[i + 1] == LfChar)
            {
                occurrences.Add(i + 1);
            }
        }

        return occurrences;
    }

    private void MoveContentToArrayStart(ref byte[] bytes, int remainingContentStart, int newContentLenght)
    {
        var remainingContentEnd = remainingContentStart + newContentLenght;
        bytes[remainingContentStart..remainingContentEnd].CopyTo(bytes, 0);
    }

    private List<string> HandleReceivedLines(List<string> lines)
    {
        List<string> messageLines = new();
        int lastParsedIndex = -1;
        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            messageLines.Add(line);
            if (line.StartsWith("<END "))
            {
                HandleReceivedMessages(new ECoSMessage(messageLines));
                messageLines = new();
                lastParsedIndex = i;
            }
        }

        if (lastParsedIndex is not -1)
        {
            lines = lines.Skip(lastParsedIndex + 1).ToList();
        }

        return lines;
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
                    Console.WriteLine($"Received reply for {message.Command}, but command was not located. Message:{message}");
                }

                break;

            default:
                throw new InvalidDataException($"ECoS Response has unknown type \"{message.Type}\"");
        }
    }
}