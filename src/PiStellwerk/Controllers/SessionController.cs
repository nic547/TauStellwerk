// <copyright file="SessionController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PiStellwerk.Data;
using PiStellwerk.Services;

namespace PiStellwerk.Controllers
{
    /// <summary>
    /// Controller handling user stuff.
    /// </summary>
    [ApiController]
    [Route("session")]
    public class SessionController : Controller
    {
        /// <summary>
        /// HTTP GET.
        /// </summary>
        /// <returns>A list of active users.</returns>
        [HttpGet("list")]
        public IReadOnlyList<Session> Get()
        {
            return SessionService.GetSessions();
        }

        [HttpPost]
        public ActionResult CreateSession([FromBody] string username)
        {
            var userAgent = Request?.Headers["User-Agent"].ToString();
            var session = SessionService.CreateSession(username, userAgent);
            return Ok(session.SessionId);
        }

        [HttpPut]
        public void Put([FromHeader(Name = "Session-Id")] string sessionId)
        {
            SessionService.TryUpdateSessionLastContact(sessionId);
        }

        [HttpPut("username")]
        public void Put([FromBody] string newUsername, [FromHeader(Name = "Session-Id")] string sessionId)
        {
            SessionService.RenameSessionUser(sessionId, newUsername);
        }
    }
}
