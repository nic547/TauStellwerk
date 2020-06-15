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
        /// HTTP PUT for renaming a user.
        /// </summary>
        /// <param name="user">A array of users, 0 being the old user, 1 the new one.</param>
        [HttpPut]
        public void Put([FromBody] User[] user)
        {
            UserService.RenameUser(user[0], user[1]);
        }
    }
}
