// <copyright file="TauJsonContext.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json.Serialization;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Base;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(EngineDto[]))]
[JsonSerializable(typeof(EngineFullDto))]
[JsonSerializable(typeof(SystemStatus))]
[JsonSerializable(typeof(string))]
public partial class TauJsonContext : JsonSerializerContext
{
}