using System.Text.Json.Serialization;
using TauStellwerk.Client.Model;

namespace TauStellwerk.WebClient;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(MutableSettings))]
public partial class SettingsJsonContext : JsonSerializerContext
{
}