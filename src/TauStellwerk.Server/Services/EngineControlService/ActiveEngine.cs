// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Base.Model;
using TauStellwerk.CommandStations;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Server.Services.EngineControlService;

public class ActiveEngine
{
    public ActiveEngine(Session session, Engine engine, EngineState state)
    {
        Session = session;
        Engine = engine;
        State = state;
    }

    public Session Session { get; set; }

    public Engine Engine { get; set; }

    public EngineState State { get; set; }
}
