// <copyright file="Program.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using PiStellwerk.Util;

namespace PiStellwerkLoadGenerator
{
    /// <summary>
    /// A tool that tries to simulate a "realistic user" by making requests to the PiStellwerk-server.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point for the console application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var options = Options.GetOptionsFromArgs(args);

            var simulators = new List<ClientSimulator>();

            for (var i = 0; i < options.Clients; i++)
            {
                var sim = new ClientSimulator(options);
                sim.StartAsync();
                simulators.Add(sim);
            }

            await Task.Delay(options.Time * 1000);

            var results = new CounterDictionary<int>();

            foreach (var sim in simulators)
            {
                sim.Stop();
                results.Combine(sim.GetStatistics());
            }

            foreach (var (key, value) in results.ToImmutableSortedDictionary())
            {
                Console.WriteLine($"{key / 10d}ms : {value} times");
            }
        }
    }
}
