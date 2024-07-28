// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;

namespace TauStellwerk.Base;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<EngineOverviewDto>[]))]
[JsonSerializable(typeof(EngineFullDto))]
[JsonSerializable(typeof(TurnoutDto))]
[JsonSerializable(typeof(List<TurnoutDto>))]
[JsonSerializable(typeof(SystemStatus))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(ResultDto))]
[JsonSerializable(typeof(ResultDto<EngineFullDto>))]
[JsonSerializable(typeof(ResultDto<int>))]
[JsonSerializable(typeof(SortEnginesBy))]
[JsonSerializable(typeof(Direction?))]
[JsonSerializable(typeof(List<BackupInfoDto>))]
public partial class TauJsonContext : JsonSerializerContext;
