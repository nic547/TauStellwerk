// <copyright file="UserController.cs" company="Dominic Ritz">
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
    public class UserController
    {
        /// <summary>
        /// HTTP GET.
        /// </summary>
        /// <returns>A list of active users.</returns>
        [HttpGet]
        public IReadOnlyList<User> Get()
        {
            return UserService.GetUsers();
        }

        /// <summary>
        /// HTTP PUT for changing one's username.
        /// </summary>
        /// <param name="newUsername">New username.</param>
        /// <param name="username">Current Username from the HTTP-Header.</param>
        /// <param name="userAgent">User-Agent from the HTTP-Header.</param>
        [HttpPut]
        public void Put([FromBody] string newUsername, [FromHeader] string username, [FromHeader(Name = "User-Agent")] string userAgent)
        {
            UserService.RenameUser(new User(username, userAgent), newUsername);
        }
    }
}
