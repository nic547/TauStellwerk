// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Base.Model;
using TauStellwerk.CommandStations;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Test.Services.EngineControlService;

public class LoggingCommandStation : CommandStationBase
{
    public List<(byte Number, State State)> EngineFunctionCalls { get; } = new();

    public override Task HandleSystemStatus(State desiredState)
    {
        return Task.CompletedTask;
    }

    public override Task HandleEngineSpeed(Engine engine, short speed, Direction priorDirection, Direction newDirection)
    {
        return Task.CompletedTask;
    }

    public override Task HandleEngineEStop(Engine engine, Direction priorDirection)
    {
        return Task.CompletedTask;
    }

    public override Task HandleEngineFunction(Engine engine, byte functionNumber, State state)
    {
        EngineFunctionCalls.Add((functionNumber, state));
        return Task.CompletedTask;
    }

    public override Task CheckState()
    {
        return Task.CompletedTask;
    }
}
