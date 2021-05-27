// <copyright file="SettingsService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PiStellwerk.Desktop.Model;

namespace PiStellwerk.Desktop.Services
{
    public class SettingsService
    {
        private const string _fileName = "settings.json";
        private Settings? _settings;

        public async Task<Settings> GetSettings(bool forceReload = false)
        {
            if (_settings == null || forceReload)
            {
                try
                {
                    await using var stream = File.OpenRead(_fileName);
                    _settings = await JsonSerializer.DeserializeAsync<Settings>(stream);
                }
                catch (Exception)
                {
                    // ignore exception, file probably just doesn't exists yet.
                }
            }

            return _settings ??= new Settings();
        }

        public async Task SetSettings(Settings settings)
        {
            _settings = settings;
            await using var stream = File.Open(_fileName, FileMode.Create);
            await JsonSerializer.SerializeAsync(stream, settings);
        }
    }
}