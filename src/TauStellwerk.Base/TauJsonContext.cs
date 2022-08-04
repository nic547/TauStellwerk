// <copyright file="TauJsonContext.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json.Serialization;

namespace TauStellwerk.Base;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(EngineOverviewDto[]))]
[JsonSerializable(typeof(EngineFullDto))]
[JsonSerializable(typeof(SystemStatus))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ResultDto<EngineFullDto>))]
public partial class TauJsonContext : JsonSerializerContext
{
}