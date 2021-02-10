// <copyright file="EngineController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiStellwerk.Commands;
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
        private const int _resultsPerPage = 20;

        private static readonly ConcurrentDictionary<int, Engine> _activeEngines = new();

        private readonly StwDbContext _dbContext;
        private readonly ICommandSystem _commandSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context for the Controller.</param>
        /// <param name="commandSystem"><see cref="ICommandSystem"/>to use.</param>
        public EngineController(StwDbContext dbContext, ICommandSystem commandSystem)
        {
            _dbContext = dbContext;
            _commandSystem = commandSystem;
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

        [HttpPost("{id}/speed/{speed}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SetEngineSpeed(int id, short speed, bool? forward)
        {
            _activeEngines.TryGetValue(id, out var engine);

            if (engine is null)
            {
                return NotFound("Engine doesn't exists or is not acquired.");
            }

            await _commandSystem.HandleEngineSpeed(engine, speed, forward);
            return Ok();
        }

        [HttpPost("{id}/function/{functionNumber}/{state}")]

        public ActionResult EngineFunction(int id, byte functionNumber, string state)
        {
            _activeEngines.TryGetValue(id, out var engine);
            if (engine is null)
            {
                return NotFound("Engine doesn't exists or is not acquired.");
            }

            _commandSystem.HandleEngineFunction(engine, functionNumber, state == "on");
            return Ok();
        }

        /// <summary>
        /// HTTP Post to acquire an engine. Needed to send commands to an engine.
        /// Only one client can have an engine acquired at a time.
        /// </summary>
        /// <param name="id">Engine to acquire.</param>
        /// <returns><see cref="ActionResult"/> indicating the success of the operation.</returns>
        [HttpPost("{id}/acquire")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public ActionResult AcquireEngine(int id)
        {
            var engine = _dbContext.Engines
                .Include(e => e.Functions)
                .Include(e => e.ECoSEngineData)
                .SingleOrDefault(e => e.Id == id);

            if (engine == null)
            {
                return NotFound("Engine not found");
            }

            if (!_commandSystem.TryAcquireEngine(engine))
            {
                return StatusCode(StatusCodes.Status423Locked, "Could not acquire Engine in CommandSystem");
            }

            if (!_activeEngines.TryAdd(engine.Id, engine))
            {
                return StatusCode(StatusCodes.Status423Locked, "Engine already acquired");
            }

            return Ok();
        }

        /// <summary>
        /// HTTP POST to release an acquired engine.
        /// </summary>
        /// <param name="id">The engine to release.</param>
        /// <returns><see cref="ActionResult"/> indicating the success of the operation.</returns>
        [HttpPost("{id}/release")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ReleaseEngine(int id)
        {
            var removalSuccess = _activeEngines.TryRemove(id, out var engine) && _commandSystem.TryReleaseEngine(engine);
            if (!removalSuccess)
            {
                return NotFound("Engine doesn't exists or is not acquired");
            }

            return Ok("Engine released successfully");
        }
    }
}
