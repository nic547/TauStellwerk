// <copyright file="IClientSettingsService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using PiStellwerk.Client.Model;

namespace PiStellwerk.Client.Services
{
    public interface IClientSettingsService
    {
        public Task<Settings> GetSettings(bool forceReload = false);

        public Task SetSettings(Settings settings);
    }
}
