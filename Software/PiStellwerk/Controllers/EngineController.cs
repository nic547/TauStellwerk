// <copyright file="EngineController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiStellwerk.Data;

namespace PiStellwerk.Controllers
{
    /// <summary>
    /// Controller for everything related to engines.
    /// </summary>
    [ApiController]
    [Route("[Controller]")]
    public class EngineController : Controller
    {
        private readonly StwDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context for the Controller.</param>
        public EngineController(StwDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// HTTP GET getting all engines.
        /// </summary>
        /// <returns>A list of engines.</returns>
        [HttpGet("List")]
        public IReadOnlyList<Engine>? GetEngines()
        {
            return _dbContext.Engines.Any() ? _dbContext.Engines.Include(e => e.Functions).ToList<Engine>() : null;
        }

        /// <summary>
        /// HTTP POST for sending a command to an Engine.
        /// Doesn't even work yet, just does a 1ms sleepy sleep.
        /// </summary>
        /// <param name="id">Id of the command, i think.</param>
        [HttpPost("command/{id}")]
        public void EngineCommand(int id)
        {
            // TODO: Remove testing sleep
            Thread.Sleep(1);
        }
    }
}
