// <copyright file="SettingsJsonContext.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json.Serialization;
using TauStellwerk.Client.Model;

namespace TauStellwerk.Client;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(MutableSettings))]
public partial class SettingsJsonContext : JsonSerializerContext
{
}