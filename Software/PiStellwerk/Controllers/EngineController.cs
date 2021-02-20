// <copyright file="EngineController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiStellwerk.Commands;
using PiStellwerk.Data;
using PiStellwerk.Services;
using PiStellwerk.Util;

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
        private readonly EngineService _engineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context for the Controller.</param>
        /// <param name="engineService">A engineService instance.</param>
        public EngineController(StwDbContext dbContext, EngineService engineService)
        {
            _dbContext = dbContext;
            _engineService = engineService;
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
        public IReadOnlyList<Engine> GetEngines(int page = 0)
        {
            return _dbContext.Engines
                .OrderByDescending(e => e.LastUsed)
                .Skip(page * _resultsPerPage)
                .Take(_resultsPerPage)
                .ToList();
        }

        [HttpPost("{id}/speed/{speed}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SetEngineSpeed([FromHeader(Name = "Session-Id")] string sessionId, int id, short speed, bool? forward)
        {
            var session = SessionService.TryGetSession(sessionId);
            if (session == null)
            {
                return BadRequest("Invalid SessionId provided.");
            }

            if (await _engineService.SetEngineSpeed(session, id, speed, forward))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("{id}/function/{functionNumber}/{state}")]

        public async Task<ActionResult> EngineFunction([FromHeader(Name = "Session-Id")] string sessionId, int id, byte functionNumber, string state)
        {
            var session = SessionService.TryGetSession(sessionId);

            if (session == null)
            {
                return BadRequest("Invalid SessionId provided.");
            }

            if (await _engineService.SetEngineFunction(session, id, functionNumber, state == "on"))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("{id}/acquire")]
        public async Task<ActionResult> AcquireEngine(int id, [FromHeader(Name = "Session-Id")] string sessionId)
        {
            var engine = _dbContext.Engines
                .Include(e => e.Functions)
                .Include(e => e.ECoSEngineData)
                .SingleOrDefault(e => e.Id == id);

            if (engine == null)
            {
                return BadRequest("Engine not found");
            }

            var session = SessionService.TryGetSession(sessionId);

            if (session == null)
            {
                return BadRequest("Session not found");
            }

            var result = await _engineService.AcquireEngine(session, engine);

            if (!result)
            {
                return StatusCode(423);
            }

            engine.LastUsed = DateTime.Now;
            _dbContext.SaveChanges();

            ConsoleService.PrintMessage($"{session.ShortSessionId}:{session.UserName} acquired {engine.Name}");

            return Ok();
        }

        [HttpPost("{id}/release")]
        public async Task<ActionResult> ReleaseEngine(int id, [FromHeader(Name = "Session-Id")] string sessionId)
        {
            var session = SessionService.TryGetSession(sessionId);
            if (session == null)
            {
                return BadRequest("Invalid SessionId provided.");
            }

            if (await _engineService.ReleaseEngine(session, id))
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
