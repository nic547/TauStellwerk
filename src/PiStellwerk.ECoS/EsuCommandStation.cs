// <copyright file="EsuCommandStation.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Net.Sockets;
using PiStellwerk.Commands;
using PiStellwerk.Data;
using PiStellwerk.Data.Commands;

#nullable enable

namespace PiStellwerk.ECoS
{
    /// <summary>
    /// Implementation of a <see cref="ICommandSystem"/> for the ESU Command Station (ECoS).
    /// </summary>
    public class EsuCommandStation: ICommandSystem
    {
        private TcpClient _client;
        private bool _systemStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="EsuCommandStation"/> class.
        /// </summary>
        public EsuCommandStation()
        {
            // TODO: Remove hardcoded IP and Address
            _client = new TcpClient();
            _client.Connect(IPAddress.Parse("192.168.1.153"), 15471);
        }


        /// <inheritdoc/>
        public void HandleEngineCommand(JsonCommand command, Engine engine)
        {
             // DO nothing
        }

        /// <inheritdoc/>
        public bool? CheckStatus()
        {
            return null;
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
    }
}
