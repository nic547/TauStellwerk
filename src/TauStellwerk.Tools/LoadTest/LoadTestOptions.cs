// <copyright file="LoadTestOptions.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using CommandLine;
using JetBrains.Annotations;

namespace TauStellwerk.Tools.LoadTest
{
    /// <summary>
    /// Class containing the options of the tool. Options come from arguments.
    /// </summary>
    [Verb("loadtest")]
    [UsedImplicitly]
    public class LoadTestOptions
    {
        public LoadTestOptions(Uri uri, bool noStats, int clients, int time)
        {
            Uri = uri;
            NoStats = noStats;
            Clients = clients;
            Time = time;
        }

        /// <summary>
        /// Gets the url of the service to test.
        /// </summary>
        [Option('u', "uri", Default = "https://localhost:5001/", HelpText = "The Uri of the PiStellwerk instance to test.")]
        public Uri Uri { get; }

        /// <summary>
        /// Gets a value indicating whether the Tool should collect latency statistics, since they might be quite memory-heavy.
        /// </summary>
        [Option("nostats", Hidden = true)]
        public bool NoStats { get; }

        /// <summary>
        /// Gets a value indicating how many clients should be simulated. Default Value: 1.
        /// </summary>
        [Option('c', "clients", Default = 1, HelpText = "How many \"clients\" should be simulated")]
        public int Clients { get; }

        /// <summary>
        /// Gets a value indicating for how many seconds the tool should run. 0/Infinite not included.
        /// </summary>
        [Option('t', "time", Default = 60, HelpText = "How long the LoadGenerator should run.")]
        public int Time { get; }
    }
}
