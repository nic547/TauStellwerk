// <copyright file="ActiveEngine.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Base;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Server.Services;

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