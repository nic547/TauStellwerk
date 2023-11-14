// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Client.Model.Settings;

namespace TauStellwerk.Client.Services;

public interface ISettingsService
{
    public delegate void SettingsChange(ImmutableSettings settings);

    public event SettingsChange SettingsChanged;

    /// <summary>
    /// Returns a immutable object with the currently active settings.
    /// </summary>
    /// <returns>A instance of <see cref="ImmutableSettings"/>.</returns>
    public Task<ImmutableSettings> GetSettings();

    public Task<MutableSettings> GetMutableSettingsCopy();

    public Task SetSettings(MutableSettings mutableSettings);
}
