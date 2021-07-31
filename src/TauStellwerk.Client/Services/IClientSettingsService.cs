// <copyright file="IClientSettingsService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using TauStellwerk.Client.Model;

namespace TauStellwerk.Client.Services
{
    public interface IClientSettingsService
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
}
