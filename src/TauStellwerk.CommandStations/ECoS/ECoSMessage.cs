// <copyright file="ECoSMessage.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.CommandStations.ECoS;

public record ECoSMessage
{
    public ECoSMessage(List<string> message)
    {
        var firstLine = message.First().Split(' ');
        var lastLine = message.Last().Split(' ');
        var messageLines = message.Skip(1).SkipLast(1);
        Content = string.Join("\r\n", messageLines);

        Type = firstLine[0].TrimStart('<');
        Command = string.Join(string.Empty, firstLine[1..]).TrimEnd('>', '\r', '\n');

        ErrorCode = int.Parse(lastLine[1]);
        ErrorMessage = string.Join(string.Empty, lastLine[2..]).TrimEnd('>', '\r', '\n');
    }

    public string Type { get; }

    public string Command { get; }

    public string Content { get; }

    public int ErrorCode { get; }

    public string ErrorMessage { get; }
}