// <copyright file="Options.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using NDesk.Options;

namespace PiStellwerkLoadGenerator
{
    /// <summary>
    /// Class containing the options of the tool. Options come from arguments.
    /// </summary>
    public class Options
    {
        private Options()
        {
        }

        /// <summary>
        /// Gets the url of the service to test.
        /// </summary>
        // TODO: "Load testing" a server in a external network by accident would be very bad. Some verification that a target is in a local Network would be nice.
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Tool should collect latency statistics, since they might be quite memory-heavy.
        /// </summary>
        public bool NoStats { get; private set; }

        /// <summary>
        /// Gets a value indicating how many clients should be simulated. Default Value: 1.
        /// </summary>
        public int Clients { get; private set; }

        /// <summary>
        /// Gets a value indicating for how many seconds the tool should run. 0/Infinite not included.
        /// </summary>
        public int Time { get; private set; }

        /// <summary>
        /// Parses the command line arguments and turns them into a <see cref="Options"/> object.
        /// </summary>
        /// <param name="args">string[] args (as received by a main function).</param>
        /// <returns><see cref="Options"/>with the parsed argument values. Otherwise default values are used.</returns>
        public static Options GetOptionsFromArgs(string[] args)
        {
            Uri uri = new Uri("http://localhost:8080/");
            var noStats = false;
            var clients = 1;
            var time = 60;

            var p = new OptionSet()
            {
                { "u|uri=", u => uri = new Uri(u) },
                { "ns|nostats=", ns => noStats = ns != null },
                { "c|clients=", (int c) => clients = c },
                { "t|time=", (int t) => time = t },
            };

            p.Parse(args);

            var options = new Options()
            {
                Uri = uri,
                NoStats = noStats,
                Clients = clients,
                Time = time,
            };

            return options;
        }
    }
}
