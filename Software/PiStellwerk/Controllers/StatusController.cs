// <copyright file="StatusController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PiStellwerk.BackgroundServices;
using PiStellwerk.Commands;
using PiStellwerk.Data;

namespace PiStellwerk.Controllers
{
    /// <summary>
    /// Controller for the status of the physical dcc output to the track.
    /// </summary>
    [ApiController]
    [Route("[Controller]")]
    public class StatusController : Controller
    {
        private const string _systemUsername = "SYSTEM";
        private static Status _status = new() { IsRunning = false, LastActionUsername = _systemUsername };

        private readonly ICommandSystem _commandSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusController"/> class.
        /// </summary>
        /// <param name="commandSystem"><see cref="ICommandSystem"/> to use.</param>
        public StatusController(ICommandSystem commandSystem)
        {
            _commandSystem = commandSystem;
        }

        [HttpPut]
        public async Task<Status> PutAsync([FromHeader(Name = "Session-Id")] string sessionId)
        {
            SessionService.UpdateSessionLastContact(sessionId);
            var systemStatus = await _commandSystem.CheckStatusAsync();
            if (systemStatus is not null && systemStatus != _status.IsRunning)
            {
                _status = new Status
                {
                    IsRunning = (bool)systemStatus,
                    LastActionUsername = _systemUsername,
                };
            }

            return _status;
        }

        /// <summary>
        /// Switch the dcc-output on or off.
        /// </summary>
        /// <param name="status">The desired new status.</param>
        [HttpPost]
        public void Post([FromBody] Status status)
        {
            _status = status;
            _commandSystem.HandleSystemStatus(status.IsRunning);
            Console.WriteLine($"{DateTime.Now} {status.LastActionUsername} has {(status.IsRunning ? "started" : "stopped")} the PiStellwerk");
        }
    }
}