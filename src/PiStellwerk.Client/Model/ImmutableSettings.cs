// <copyright file="ImmutableSettings.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace PiStellwerk.Client.Model
{
    public class ImmutableSettings
    {
        public ImmutableSettings(string username, string serverAddress)
        {
            Username = username;
            ServerAddress = serverAddress;
        }

        public string Username { get; }

        public string ServerAddress { get; }
    }
}