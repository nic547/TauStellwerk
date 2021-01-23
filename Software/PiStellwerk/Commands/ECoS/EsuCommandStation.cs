// <copyright file="EsuCommandStation.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Net;
using System.Threading.Tasks;
using PiStellwerk.Data;
using PiStellwerk.Data.Commands;

#nullable enable

namespace PiStellwerk.Commands.ECoS
{
    /// <summary>
    /// Implementation of a <see cref="ICommandSystem"/> for the ESU Command Station (ECoS).
    /// </summary>
    public class EsuCommandStation : ICommandSystem
    {
        private readonly ECosConnectionHandler _connectionHandler;

        private bool _systemStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="EsuCommandStation"/> class.
        /// </summary>
        public EsuCommandStation()
        {
            // TODO: Remove hardcoded IP and Address
             _connectionHandler = new ECosConnectionHandler(IPAddress.Parse("192.168.1.153"), 15471);

             _ = _connectionHandler.RegisterEvent("request(1,view)", "1", HandleStatusEvent);
        }

        /// <inheritdoc/>
        public void HandleEngineCommand(JsonCommand command, Engine engine)
        {
             // DO nothing
        }

        /// <inheritdoc/>
        public Task<bool?> CheckStatusAsync()
        {
            return Task.FromResult<bool?>(_systemStatus);
        }

        /// <inheritdoc/>
        public bool TryAcquireEngine(Engine engine)
        {
            return true;
        }

        /// <inheritdoc/>
        public bool TryReleaseEngine(Engine engine)
        {
            return true;
        }

        private void HandleStatusEvent(string message)
        {
            _systemStatus = message.Contains("status[GO]");
        }
    }
}
