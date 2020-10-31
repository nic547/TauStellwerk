// <copyright file="ClientActionBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PiStellwerkLoadGenerator.ClientActions
{
    /// <summary>
    /// A action performed by a client.
    /// </summary>
    public abstract class ClientActionBase
    {
        /// <summary>
        /// Gets the interval in which the actions should be performed.
        /// </summary>
        public abstract int Interval { get; }

        /// <summary>
        /// Gets the HttpClient to use.
        /// </summary>
        internal HttpClient Client { get; private set; }

        /// <summary>
        /// Gets the Options to use.
        /// </summary>
        internal Options Options { get; private set; }

        /// <summary>
        /// Gets an instance of <see cref="Random"/> to use.
        /// </summary>
        internal Random Random { get; private set; }

        /// <summary>
        /// Initialize the stuff needed.
        /// </summary>
        /// <param name="client">The HTTPClient to use.</param>
        /// <param name="options">The Options to use.</param>
        /// <param name="random">A instance of <see cref="System.Random"/> to use.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public virtual Task Initialize(HttpClient client, Options options, Random random)
        {
            Client = client;
            Options = options;
            Random = random;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Perform the action, send the http request to the server.
        /// </summary>
        /// <returns>Time until response was received.</returns>
        public abstract Task<int> PerformRequest();
    }
}
