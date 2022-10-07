// <copyright file="ImmutableSettings.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Client.Model;

public class ImmutableSettings
{
    public ImmutableSettings(string username, string serverAddress, string theme, bool wakeLock)
    {
        Username = username;
        ServerAddress = serverAddress;
        Theme = theme;
        WakeLock = wakeLock;
    }

    public string Username { get; }

    public string ServerAddress { get; }

    public string Theme { get; }

    public bool WakeLock { get; }
}