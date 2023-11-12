// <copyright file="ECoSMessageDecoder.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Text.RegularExpressions;

namespace TauStellwerk.Server.CommandStations;

/// <summary>
/// Provides methods that decode a message from the ECoS into something "more usable".
/// </summary>
public static partial class ECoSMessageDecoder
{
    private static readonly Regex _funcDescRegex = FuncDescRegex();
    private static readonly Regex _engineListRegex = EngineListRegex();

    public static IEnumerable<(byte Number, short Type, bool IsMomentary)> DecodeFuncdescMessage(string message)
    {
        var lines = message.Split('\n');

        foreach (var line in lines)
        {
            var match = _funcDescRegex.Match(line);
            if (match.Success)
            {
                var functionId = byte.Parse(match.Groups["functionId"].Value);
                var functionType = short.Parse(match.Groups["functionDesc"].Value);
                var isMomentary = match.Groups["momentary"].Value == "moment";

                yield return (functionId, functionType, isMomentary);
            }
        }
    }

    public static IEnumerable<(int Id, string Name, string Protocol)> DecodeEngineListMessage(string message)
    {
        var lines = message.Split("\r\n");

        foreach (var line in lines)
        {
            var match = _engineListRegex.Match(line);
            if (match.Success)
            {
                var id = int.Parse(match.Groups["Id"].Value);
                var name = match.Groups["Name"].Value;
                var protocol = match.Groups["Protocol"].Value;

                yield return (id, name, protocol);
            }
        }
    }

    [GeneratedRegex("(?:\\d+) funcdesc\\[(?<functionId>\\d+),(?<functionDesc>\\d+),?(?<momentary>.*)\\]", RegexOptions.Compiled)]
    private static partial Regex FuncDescRegex();

    [GeneratedRegex("(?<Id>\\d*) name\\[\"(?<Name>.*?)\"\\] protocol\\[(?<Protocol>.*?)\\]", RegexOptions.Compiled)]
    private static partial Regex EngineListRegex();
}