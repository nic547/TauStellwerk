// <copyright file="SessionController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

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
            var sessionId = HttpContext.Session.Id;
            HttpContext.Session.Set("is-set", new byte[1]); // just needed to "get the session to stick"
            SessionService.CreateSession(username, userAgent, sessionId);
            return Ok();
        }

        /// <summary>
        /// HTTP PUT for changing one's username.
        /// </summary>
        /// <param name="newUsername">New username.</param>
        [HttpPut]
        public void Put([FromBody] string newUsername)
        {
            SessionService.RenameSessionUser(HttpContext.Session.Id, newUsername);
        }
    }
}
