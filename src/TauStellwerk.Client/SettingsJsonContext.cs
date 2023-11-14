// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;
using TauStellwerk.Client.Model.Settings;

namespace TauStellwerk.Client;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(MutableSettings))]
public partial class SettingsJsonContext : JsonSerializerContext
{
}
