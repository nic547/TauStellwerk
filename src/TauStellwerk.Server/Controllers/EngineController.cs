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
using TauStellwerk.Base.Model;
using TauStellwerk.Database;
using TauStellwerk.Database.Model;
using TauStellwerk.Services;
using TauStellwerk.Util;

namespace TauStellwerk.Controllers
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
        private readonly SessionService _sessionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context for the Controller.</param>
        /// <param name="engineService">A engineService instance.</param>
        /// <param name="sessionService">A sessionService instance.</param>
        public EngineController(StwDbContext dbContext, IEngineService engineService, SessionService sessionService)
        {
            _dbContext = dbContext;
            _engineService = engineService;
            _sessionService = sessionService;
        }

        /// <summary>
        /// Get a single engine by it's id.
        /// </summary>
        /// <param name="id">The id of the engine.</param>
        /// <returns>The engine with the given id.</returns>
        [HttpGet("{id:int}")]
        public async Task<EngineFullDto?> GetEngine(int id)
        {
            return await _dbContext.Engines
                .AsNoTracking()
                .AsSingleQuery()
                .Include(x => x.Functions)
                .Include(x => x.Images)
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Id == id)
                .ContinueWith(x => x?.Result?.ToEngineFullDto());
        }

        /// <summary>
        /// HTTP GET getting all engines.
        /// Results are paginated.
        /// </summary>
        /// <param name="page">Page to load. Default and start is Zero.</param>
        /// <param name="showHiddenEngines">Whether engines that are hidden should be included.</param>
        /// <param name="sortBy">Should engines be sorted by LastUsed or Created?.</param>
        /// <param name="sortDescending">Should Engines be sorted descending (or ascending).</param>
        /// <returns>A list of engines.</returns>
        [HttpGet("List")]
        public async Task<IList<EngineDto>> GetEngines(int page = 0, bool showHiddenEngines = false, SortEnginesBy sortBy = SortEnginesBy.LastUsed, bool sortDescending = true)
        {
            var query = _dbContext.Engines
               .AsNoTracking()
               .AsSplitQuery();

            if (!showHiddenEngines)
            {
                query = query.Where(e => !e.IsHidden);
            }

            query = (sortBy, sortDescending) switch
            {
                (SortEnginesBy.Created, false) => query.OrderBy(e => e.Created),
                (SortEnginesBy.Created, true) => query.OrderByDescending(e => e.Created),
                (SortEnginesBy.Name, false) => query.OrderBy(e => e.Name),
                (SortEnginesBy.Name, true) => query.OrderByDescending(e => e.Name),
                (SortEnginesBy.LastUsed, false) => query.OrderBy(e => e.LastUsed),
                (SortEnginesBy.LastUsed, true) => query.OrderByDescending(e => e.LastUsed),
                _ => throw new InvalidOperationException(),
            };

            query = query.Skip(page * _resultsPerPage)
            .Take(_resultsPerPage)
            .Include(e => e.Images)
            .Include(x => x.Tags);

            var result = await query.ToListAsync();
            return result.Select(e => e.ToEngineDto()).ToList();
        }

        /// <summary>
        /// Add or update an engine.
        /// </summary>
        /// <param name="engineDto">The engine to add or update.</param>
        /// <returns>The updated engine.</returns>
        [HttpPost]
        public async Task<ActionResult<EngineFullDto>> UpdateOrAdd(EngineFullDto engineDto)
        {
            Engine? engine;
            if (engineDto.Id == 0)
            {
                engine = new Engine();
                _dbContext.Engines.Add(engine);
            }
            else
            {
                engine = await _dbContext.Engines
                    .Include(x => x.Functions)
                    .Include(x => x.Tags)
                    .SingleOrDefaultAsync(x => x.Id == engineDto.Id);

                if (engine == null)
                {
                    return NotFound();
                }
            }

            await engine.UpdateWith(engineDto, _dbContext);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                ConsoleService.PrintError($"Exception while updating engine: {e.GetType()}");
                return UnprocessableEntity();
            }

            engineDto.Id = engine.Id;

            return engineDto;
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var engine = await _dbContext.Engines
                .Include(e => e.Images)
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

        /// <summary>
        /// Set the Speed of an Engine.
        /// Requires having acquired control of an Engine.
        /// </summary>
        /// <param name="sessionId">Valid session Id.</param>
        /// <param name="id">Id of the engine.</param>
        /// <param name="speed">Desired Speed.</param>
        /// <param name="forward">Optional parameter indicating the direction.</param>
        /// <returns>Response indicating success or failure.</returns>
        [HttpPost("{id:int}/speed/{speed}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SetEngineSpeed([FromHeader(Name = "Session-Id")] string sessionId, int id, short speed, bool? forward)
        {
            var session = _sessionService.TryGetSession(sessionId);
            if (session == null)
            {
                return StatusCode(403);
            }

            if (await _engineService.SetEngineSpeed(session, id, speed, forward))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("{id:int}/estop")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SetEngineEStop([FromHeader(Name = "Session-Id")] string sessionId, int id)
        {
            var session = _sessionService.TryGetSession(sessionId);
            if (session == null)
            {
                return StatusCode(403);
            }

            if (await _engineService.SetEngineEStop(session, id))
            {
                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Activate/Deactivate a function of an engine.
        /// </summary>
        /// <param name="sessionId">A Valid session Id.</param>
        /// <param name="id">Id of the engine.</param>
        /// <param name="functionNumber">The function.</param>
        /// <param name="state">On or off.</param>
        /// <returns>Response indicating success or failure.</returns>
        [HttpPost("{id:int}/function/{functionNumber}/{state}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> EngineFunction([FromHeader(Name = "Session-Id")] string sessionId, int id, byte functionNumber, string state)
        {
            var session = _sessionService.TryGetSession(sessionId);

            if (session == null)
            {
                return StatusCode(403);
            }

            if (await _engineService.SetEngineFunction(session, id, functionNumber, state == "on"))
            {
                return Ok();
            }

            return BadRequest();
        }

        /// <summary>
        /// Acquire control of an engine.
        /// </summary>
        /// <param name="id">Id of the Engine.</param>
        /// <param name="sessionId">Valid session Id.</param>
        /// <returns>Response indicating success.</returns>
        [HttpPost("{id:int}/acquire")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
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

            var session = _sessionService.TryGetSession(sessionId);

            if (session == null)
            {
                return StatusCode(403);
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

        /// <summary>
        /// Release command of an engine.
        /// </summary>
        /// <param name="id">Id of the engine.</param>
        /// <param name="sessionId">SessionId.</param>
        /// <returns>Response indicating whether the operation was successful.</returns>
        [HttpPost("{id:int}/release")]
        public async Task<ActionResult> ReleaseEngine(int id, [FromHeader(Name = "Session-Id")] string sessionId)
        {
            var session = _sessionService.TryGetSession(sessionId);
            if (session == null)
            {
                return StatusCode(403);
            }

            if (!await _engineService.ReleaseEngine(session, id))
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
