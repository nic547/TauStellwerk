// <copyright file="IEngineService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using TauStellwerk.Base.Model;
using TauStellwerk.Commands;
using TauStellwerk.Database.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Services
{
    public interface IEngineService
    {
        Task<bool> AcquireEngine(Session session, Engine engine);

        Task<bool> ReleaseEngine(Session session, int engineId);

        Task<bool> SetEngineSpeed(Session session, int engineId, int speed, bool? shouldDriveForward);

        Task<bool> SetEngineEStop(Session session, int engineId);

        Task<bool> SetEngineFunction(Session session, int engineId, int functionNumber, bool on);
    }

    public class EngineService : IEngineService
    {
        private readonly CommandSystemBase _commandSystem;

        private readonly ConcurrentDictionary<int, ActiveEngine> _activeEngines = new();

        public EngineService(CommandSystemBase commandSystem, SessionService sessionService)
        {
            _commandSystem = commandSystem;
            sessionService.SessionTimeout += HandleSessionTimeout;
        }

        public async Task<bool> AcquireEngine(Session session, Engine engine)
        {
            var result = _activeEngines.TryAdd(engine.Id, new ActiveEngine(session, engine));
            if (!result)
            {
                ConsoleService.PrintMessage($"{session} tried acquiring {engine}, but engine is already acquired.");
                return result;
            }

            var systemResult = await _commandSystem.TryAcquireEngine(engine);

            if (systemResult == false)
            {
                ConsoleService.PrintWarning($"{session} tried acquiring {engine}, but ICommandSystem returned false");
                _activeEngines.TryRemove(engine.Id, out _);
                return false;
            }

            ConsoleService.PrintMessage($"{session} acquired {engine}");

            return true;
        }

        public async Task<bool> ReleaseEngine(Session session, int engineId)
        {
            if (!IsValidEngineId(engineId, out var activeEngine, session) || !IsCorrectSession(session, activeEngine))
            {
                return false;
            }

            _activeEngines.TryRemove(engineId, out _);
            ConsoleService.PrintMessage($"{session} released {activeEngine.Engine}");

            return await _commandSystem.TryReleaseEngine(activeEngine.Engine);
        }

        public async Task<bool> SetEngineSpeed(Session session, int engineId, int speed, bool? shouldBeDrivingForwards)
        {
            if (!IsValidEngineId(engineId, out var activeEngine, session) || !IsCorrectSession(session, activeEngine))
            {
                return false;
            }

            var hasBeenDrivingForward = activeEngine.IsDrivingForward;
            shouldBeDrivingForwards ??= hasBeenDrivingForward;
            activeEngine.IsDrivingForward = (bool)shouldBeDrivingForwards;

            await _commandSystem.HandleEngineSpeed(activeEngine.Engine, (short)speed, hasBeenDrivingForward, (bool)shouldBeDrivingForwards);
            return true;
        }

        public async Task<bool> SetEngineEStop(Session session, int engineId)
        {
            if (!IsValidEngineId(engineId, out var activeEngine, session) || !IsCorrectSession(session, activeEngine))
            {
                return false;
            }

            var hasBeenDrivingForward = activeEngine.IsDrivingForward;
            await _commandSystem.HandleEngineEStop(activeEngine.Engine, hasBeenDrivingForward);
            return true;
        }

        public async Task<bool> SetEngineFunction(Session session, int engineId, int functionNumber, bool on)
        {
            if (!IsValidEngineId(engineId, out var activeEngine, session) || !IsCorrectSession(session, activeEngine))
            {
                return false;
            }

            await _commandSystem.HandleEngineFunction(activeEngine.Engine, (byte)functionNumber, on);
            return true;
        }

        private bool IsValidEngineId(int engineId, [NotNullWhen(true)] out ActiveEngine? activeEngine, Session session)
        {
            _activeEngines.TryGetValue(engineId, out activeEngine);
            if (activeEngine == null)
            {
                ConsoleService.PrintMessage($"{session} tried commanding engine with id {engineId}, but no such engine is active.");
                return false;
            }

            return true;
        }

        private bool IsCorrectSession(Session session, ActiveEngine? activeEngine)
        {
            if (activeEngine?.Session != session)
            {
                ConsoleService.PrintWarning($"{session} tried releasing {activeEngine?.Engine}, but has wrong session");
                return false;
            }

            return true;
        }

        private void HandleSessionTimeout(Session session)
        {
            foreach (var active in _activeEngines.Values)
            {
                if (active.Session == session)
                {
                    _activeEngines.TryRemove(active.Engine.Id, out var _);
                    ConsoleService.PrintWarning($"Released {active.Engine} because {session.UserName} timed out!");
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

            public bool IsDrivingForward { get; set; } = true;
        }
    }
}
