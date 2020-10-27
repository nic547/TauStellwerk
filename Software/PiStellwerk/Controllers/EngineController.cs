// <copyright file="EngineController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiStellwerk.Commands;
using PiStellwerk.Data;
using PiStellwerk.Data.Commands;

namespace PiStellwerk.Controllers
{
    /// <summary>
    /// Controller for everything related to engines.
    /// </summary>
    [ApiController]
    [Route("[Controller]")]
    public class EngineController : Controller
    {
        private const int _resultsPerPage = 20;
        private readonly StwDbContext _dbContext;
        private readonly ICommandSystem _commandSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context for the Controller.</param>
        public EngineController(StwDbContext dbContext)
        {
            _dbContext = dbContext;
            _commandSystem = new ConsoleCommandSystem(); // TODO: DI this.
        }

        /// <summary>
        /// Get a single engine by it's id.
        /// </summary>
        /// <param name="id">The id of the engine.</param>
        /// <returns>The engine with the given id.</returns>
        [HttpGet("{id}")]
        public Engine GetEngine(int id)
        {
            return _dbContext.Engines.Include(x => x.Functions).Single(x => x.Id == id);
        }

        /// <summary>
        /// HTTP GET getting all engines.
        /// Results are paginated.
        /// </summary>
        /// <param name="page">Page to load. Default and start is Zero.</param>
        /// <returns>A list of engines.</returns>
        [HttpGet("List")]
        public IReadOnlyList<Engine>? GetEngines(int page = 0)
        {
            return _dbContext.Engines
                .Skip(page * _resultsPerPage)
                .Take(_resultsPerPage)
                .ToList();
        }

        /// <summary>
        /// HTTP POST for sending a command to an Engine.
        /// Doesn't even work yet, just does a 1ms sleepy sleep.
        /// </summary>
        /// <param name="id">Id of the engine.</param>
        /// <param name="command"><see cref="JsonCommand"/>.</param>
        [HttpPost("{id}/command")]
        public void EngineCommand(int id, JsonCommand command)
        {
            var engine = _dbContext.Engines.Single(e => e.Id == id);
            _commandSystem.HandleCommand(command, engine);
        }
    }
}
