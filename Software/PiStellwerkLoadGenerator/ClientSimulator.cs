// <copyright file="ClientSimulator.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Timers;
using PiStellwerk.Data;
using PiStellwerk.Util;
using PiStellwerkLoadGenerator.ClientActions;

namespace PiStellwerkLoadGenerator
{
    /// <summary>
    /// Simulates a single client that makes it's HTTP Requests.
    /// </summary>
    public class ClientSimulator
    {
        private readonly Options _options;
        private readonly Dictionary<int, ulong> _results = new Dictionary<int, ulong>();

        private readonly User _user;

        private readonly List<Timer> _timers = new List<Timer>();

        private readonly Random _random = new Random();
        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSimulator"/> class.
        /// </summary>
        /// <param name="options"><see cref="Options"/>.</param>
        public ClientSimulator(Options options)
        {
            _options = options;

            _user = new User()
            {
                Name = _random.Next(1, 999999999).ToString(),
                UserAgent = "PiStellwerk ",
            };
        }

        /// <summary>
        /// Starts the client simulation.
        /// </summary>
        public void Start()
        {
            var heartbeatTimer = new Timer(2000);
            heartbeatTimer.Elapsed += async (s, e) => { _results.IncrementValue(await Heartbeat.PerformRequest(_options, _user, _httpClient)); };
            heartbeatTimer.Start();
            _timers.Add(heartbeatTimer);
        }

        /// <summary>
        /// Stops the ClientSimulation.
        /// </summary>
        public void Stop()
        {
            foreach (var timer in _timers)
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        /// <summary>
        /// Get the latency results of this ClientSimulator.
        /// </summary>
        /// <returns>A dictonary, containing the latency in 1/10ms as Key and the number of occurrences as Value.</returns>
        public IReadOnlyDictionary<int, ulong> GetStatistics()
        {
            return _results;
        }
    }
}
