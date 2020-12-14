// <copyright file="StatusController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Microsoft.AspNetCore.Mvc;
using PiStellwerk.BackgroundServices;
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
        private static Status _status = new() { IsRunning = false, LastActionUsername = "SYSTEM" };

        /// <summary>
        /// HTTP PUT handling the heartbeat and sending the current status of the actual dcc-output.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Current <see cref="Status"/>.</returns>
        [HttpPut]
        public Status Put([FromBody] User user)
        {
            UserService.UpdateUser(user);
            return _status;
        }

        /// <summary>
        /// Switch the decc-output on or off.
        /// </summary>
        /// <param name="status">The desired new status.</param>
        [HttpPost]
        public void Post([FromBody] Status status)
        {
            _status = status;
            Console.WriteLine($"{DateTime.Now} {status.LastActionUsername} has {(status.IsRunning ? "started" : "stopped")} the PiStellwerk");
        }
    }
}