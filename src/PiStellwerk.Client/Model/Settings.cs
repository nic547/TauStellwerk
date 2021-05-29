// <copyright file="Settings.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace PiStellwerk.Client.Model
{
    public class Settings
    {
        public string Username { get; set; } = $"Random User {new Random().Next(999_999)}";

        public string ServerAddress { get; set; } = "https://localhost";
    }
}