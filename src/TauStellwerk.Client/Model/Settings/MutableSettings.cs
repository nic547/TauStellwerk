// <copyright file="MutableSettings.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Client.Model;

public class MutableSettings
{
    public string Username { get; set; } = UsernameGenerator.GetRandomUsername();

    public string ServerAddress { get; set; } = "https://localhost";

    public string Theme { get; set; } = "light";

    public ImmutableSettings GetImmutableCopy()
    {
        return new ImmutableSettings(
            Username,
            ServerAddress,
            Theme);
    }

    public MutableSettings GetMutableCopy()
    {
        return new MutableSettings()
        {
            Username = Username,
            ServerAddress = ServerAddress,
            Theme = Theme,
        };
    }
}