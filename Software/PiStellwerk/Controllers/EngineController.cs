// <copyright file="EngineController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Collections.Concurrent;
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

        private static readonly ConcurrentDictionary<int, ActiveEngine> _activeEngines = new();

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

            SessionService.SessionTimeout += HandleSessionTimeout;
        }

        public static void ClearActiveEngines()
        {
            // Separate into non-static EngineService
            _activeEngines.Clear();
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
            var result = GetActiveEngine(sessionId, id);
            if (result.Result is not OkObjectResult)
            {
                return result.Result;
            }

            await _commandSystem.HandleEngineSpeed(result.Value, speed, forward);
            return Ok();
        }

        [HttpPost("{id}/function/{functionNumber}/{state}")]

        public ActionResult EngineFunction([FromHeader(Name = "Session-Id")] string sessionId, int id, byte functionNumber, string state)
        {
            var result = GetActiveEngine(sessionId, id);
            if (result.Result != Ok())
            {
                return result.Result;
            }

            _commandSystem.HandleEngineFunction(result.Value, functionNumber, state == "on");
            return Ok();
        }

        [HttpPost("{id}/acquire")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status423Locked)]
        public ActionResult AcquireEngine(int id, [FromHeader(Name = "Session-Id")] string sessionId)
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

            var session = SessionService.TryGetSession(sessionId);

            if (session == null)
            {
                return Unauthorized("No session found");
            }

            var activeEngine = new ActiveEngine(session, engine);

            if (!_activeEngines.TryAdd(engine.Id, activeEngine))
            {
                return StatusCode(StatusCodes.Status423Locked, "Engine already acquired");
            }

            engine.LastUsed = DateTime.Now;
            _dbContext.SaveChanges();

            ConsoleService.PrintMessage($"Engine {engine.Name} acquired by {session.UserName}");

            return Ok();
        }

        [HttpPost("{id}/release")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ReleaseEngine(int id, [FromHeader(Name = "Session-Id")] string sessionId)
        {
            var result = GetActiveEngine(sessionId, id);

            if (result.Result is not OkObjectResult)
            {
                return result.Result;
            }

            var removalSuccess = _activeEngines.TryRemove(id, out var activeEngine) && _commandSystem.TryReleaseEngine(activeEngine.Engine);
            if (!removalSuccess)
            {
                return NotFound("Engine doesn't exists or is not acquired");
            }

            return Ok("Engine released successfully");
        }

        private static void HandleSessionTimeout(Session session)
        {
            foreach (var active in _activeEngines.Values)
            {
                if (active.Session == session)
                {
                    _activeEngines.TryRemove(active.Engine.Id, out var _);
                    ConsoleService.PrintWarning($"Released {active.Engine.Name} because {session.UserName} timed out!");
                }
            }
        }

        private ActionResult<Engine> GetActiveEngine(string sessionId, int id)
        {
            _ = _activeEngines.TryGetValue(id, out var activeEngine);
            var session = SessionService.TryGetSession(sessionId);

            if (session is null)
            {
                ConsoleService.PrintWarning($"Invalid session tried to interact with engine id {id}, SessionId {sessionId}");
                return Unauthorized(null);
            }

            if (activeEngine is null)
            {
                ConsoleService.PrintWarning($"{session.ShortSessionId}:{session.UserName} tried to interact with engine id {id}, but no such engine is active");
                return NotFound(null);
            }

            if (activeEngine.Session.SessionId != sessionId)
            {
                ConsoleService.PrintWarning($"{session.ShortSessionId}:{session.UserName} tried to interact with {activeEngine.Engine.Name}, but has the wrong session.");
                return Unauthorized(null);
            }

            return Ok(activeEngine.Engine);
        }

        private class ActiveEngine
        {
            public ActiveEngine(Session session, Engine engine)
            {
                Session = session;
                Engine = engine;
            }

            public Session Session { get; }

            public Engine Engine { get; }
        }
    }
}
