// <copyright file="EngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Collections.Concurrent;
using System.Threading.Tasks;
using PiStellwerk.Commands;
using PiStellwerk.Data;
using PiStellwerk.Util;

namespace PiStellwerk.Services
{
    public class EngineService
    {
        private readonly ICommandSystem _commandSystem;

        private readonly ConcurrentDictionary<int, ActiveEngine> _activeEngines = new();

        public EngineService(ICommandSystem commandSystem)
        {
            _commandSystem = commandSystem;
            SessionService.SessionTimeout += HandleSessionTimeout;
        }

        public async Task<bool> AcquireEngine(Session session, Engine engine)
        {
            var result = _activeEngines.TryAdd(engine.Id, new ActiveEngine(session, engine));
            if (!result)
            {
                return result;
            }

            var systemResult = await _commandSystem.TryAcquireEngine(engine);

            if (systemResult == false)
            {
                _activeEngines.TryRemove(engine.Id, out _);
            }

            return systemResult;
        }

        public async Task<bool> ReleaseEngine(Session session, int engineId)
        {
            _activeEngines.TryGetValue(engineId, out var activeEngine);

            if (activeEngine?.Session != session)
            {
                return false;
            }

            _activeEngines.TryRemove(engineId, out _);
            return await _commandSystem.TryReleaseEngine(activeEngine.Engine);
        }

        public async Task<bool> SetEngineSpeed(Session session, int engineId, int speed, bool? forward)
        {
            _activeEngines.TryGetValue(engineId, out var activeEngine);

            if (activeEngine?.Session != session)
            {
                return false;
            }

            await _commandSystem.HandleEngineSpeed(activeEngine.Engine, (short)speed, forward);
            return true;
        }

        public async Task<bool> SetEngineFunction(Session session, int engineId, int functionNumber, bool on)
        {
            _activeEngines.TryGetValue(engineId, out var activeEngine);

            if (activeEngine?.Session != session)
            {
                return false;
            }

            await _commandSystem.HandleEngineFunction(activeEngine.Engine, (byte)functionNumber, on);
            return true;
        }

        private void HandleSessionTimeout(Session session)
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
