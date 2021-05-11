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
using PiStellwerk.Data;
using PiStellwerk.Database;
using PiStellwerk.Services;
using PiStellwerk.Util;

namespace PiStellwerk.Controllers
{
    /// <summary>
    /// Controller for everything related to engines.
    /// </summary>
    [ApiController]
    [Route("engine")]
    public class EngineController : Controller
    {
        private const int _resultsPerPage = 20;

        private readonly StwDbContext _dbContext;
        private readonly IEngineService _engineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context for the Controller.</param>
        /// <param name="engineService">A engineService instance.</param>
        public EngineController(StwDbContext dbContext, IEngineService engineService)
        {
            _dbContext = dbContext;
            _engineService = engineService;
        }

        /// <summary>
        /// Get a single engine by it's id.
        /// </summary>
        /// <param name="id">The id of the engine.</param>
        /// <returns>The engine with the given id.</returns>
        [HttpGet("{id:int}")]
        public async Task<Engine> GetEngine(int id)
        {
            return await _dbContext.Engines
                .AsNoTracking()
                .Include(x => x.Functions)
                .Include(x => x.Image)
                .SingleAsync(x => x.Id == id);
        }

        /// <summary>
        /// HTTP GET getting all engines.
        /// Results are paginated.
        /// </summary>
        /// <param name="page">Page to load. Default and start is Zero.</param>
        /// <returns>A list of engines.</returns>
        [HttpGet("List")]
        public async Task<IReadOnlyList<Engine>> GetEngines(int page = 0)
        {
            var test = await _dbContext.Engines
                .AsNoTracking()
                .OrderByDescending(e => e.LastUsed)
                .Skip(page * _resultsPerPage)
                .Take(_resultsPerPage)
                .Include(e => e.Image)
                .ToListAsync();
            return test;
        }

        /// <summary>
        /// Add or update an engine.
        /// </summary>
        /// <param name="engine">The engine to add or update.</param>
        /// <returns>The updated engine.</returns>
        [HttpPost]
        public async Task<ActionResult<Engine>> UpdateOrAdd(Engine engine)
        {
            _dbContext.Update(engine);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                ConsoleService.PrintError($"Exception while updating engine: {e.GetType()}");
                return UnprocessableEntity();
            }

            return engine;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var engine = await _dbContext.Engines
                .Include(e => e.Image)
                .Include(e => e.Functions)
                .SingleOrDefaultAsync(e => e.Id == id);
            if (engine == null)
            {
                return NotFound();
            }

            _dbContext.Engines.Remove(engine);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id:int}/speed/{speed}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SetEngineSpeed([FromHeader(Name = "Session-Id")] string sessionId, int id, short speed, bool? forward)
        {
            var session = SessionService.TryGetSession(sessionId);
            if (session == null)
            {
                return Forbid();
            }

            if (await _engineService.SetEngineSpeed(session, id, speed, forward))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("{id:int}/function/{functionNumber}/{state}")]

        public async Task<ActionResult> EngineFunction([FromHeader(Name = "Session-Id")] string sessionId, int id, byte functionNumber, string state)
        {
            var session = SessionService.TryGetSession(sessionId);

            if (session == null)
            {
                return Forbid();
            }

            if (await _engineService.SetEngineFunction(session, id, functionNumber, state == "on"))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("{id:int}/acquire")]
        public async Task<ActionResult> AcquireEngine(int id, [FromHeader(Name = "Session-Id")] string sessionId)
        {
            var engine = await _dbContext.Engines
                .Include(e => e.Functions)
                .Include(e => e.ECoSEngineData)
                .SingleOrDefaultAsync(e => e.Id == id);

            if (engine == null)
            {
                return NotFound();
            }

            var session = SessionService.TryGetSession(sessionId);

            if (session == null)
            {
                return Forbid();
            }

            var result = await _engineService.AcquireEngine(session, engine);

            if (!result)
            {
                return StatusCode(423);
            }

            engine.LastUsed = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id:int}/release")]
        public async Task<ActionResult> ReleaseEngine(int id, [FromHeader(Name = "Session-Id")] string sessionId)
        {
            var session = SessionService.TryGetSession(sessionId);
            if (session == null)
            {
                return Forbid();
            }

            if (!await _engineService.ReleaseEngine(session, id))
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
