// <copyright file="SessionController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PiStellwerk.BackgroundServices;
using PiStellwerk.Data;

namespace PiStellwerk.Controllers
{
    /// <summary>
    /// Controller handling user stuff.
    /// </summary>
    [ApiController]
    [Route("[Controller]")]
    public class SessionController : Controller
    {
        /// <summary>
        /// HTTP GET.
        /// </summary>
        /// <returns>A list of active users.</returns>
        [HttpGet]
        public IReadOnlyList<Session> Get()
        {
            return SessionService.GetSessions();
        }

        [HttpPost]
        public ActionResult CreateSession([FromBody] string username, [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var session = SessionService.CreateSession(username, userAgent);
            Console.WriteLine($"New Session created by {session.UserName}");
            return Ok(session.SessionId);
        }

        [HttpPut]
        public void Put([FromBody] string newUsername, [FromHeader(Name = "Session-Id")] string sessionId)
        {
            SessionService.RenameSessionUser(sessionId, newUsername);
        }
    }
}
