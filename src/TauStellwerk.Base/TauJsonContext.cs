// <copyright file="TauJsonContext.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json.Serialization;

namespace TauStellwerk.Base;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<EngineOverviewDto>[]))]
[JsonSerializable(typeof(EngineFullDto))]
[JsonSerializable(typeof(TurnoutDto))]
[JsonSerializable(typeof(List<TurnoutDto>))]
[JsonSerializable(typeof(SystemStatus))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ResultDto<EngineFullDto>))]
[JsonSerializable(typeof(ResultDto))]
[JsonSerializable(typeof(SortEnginesBy))]
[JsonSerializable(typeof(Direction?))]
public partial class TauJsonContext : JsonSerializerContext
{
}