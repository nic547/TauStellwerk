// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Client.Model.Settings;

public class MutableSettings
{
    public string Username { get; set; } = UsernameGenerator.GetRandomUsername();

    public string ServerAddress { get; set; } = "https://localhost";

    public string Theme { get; set; } = "Light";

    public bool WakeLock { get; set; }

    public string Language { get; set; } = "English";

    public ImmutableSettings GetImmutableCopy()
    {
        return new ImmutableSettings(
            Username,
            ServerAddress,
            Theme,
            WakeLock,
            Language);
    }

    public MutableSettings GetMutableCopy()
    {
        return new MutableSettings()
        {
            Username = Username,
            ServerAddress = ServerAddress,
            Theme = Theme,
            WakeLock = WakeLock,
            Language = Language,
        };
    }
}
