// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Client.Model.Settings;

public class ImmutableSettings
{
    public ImmutableSettings(string username, string serverAddress, string theme, bool wakeLock, string language)
    {
        Username = username;
        ServerAddress = serverAddress;
        Theme = theme;
        WakeLock = wakeLock;
        Language = language;
    }

    public string Username { get; }

    public string ServerAddress { get; }

    public string Theme { get; }

    public bool WakeLock { get; }

    public string Language { get; }
}
