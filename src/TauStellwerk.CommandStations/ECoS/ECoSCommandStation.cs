﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TauStellwerk.Base.Model;
using TauStellwerk.Data;
using TauStellwerk.Data.Model;
using TauStellwerk.Server.CommandStations;

namespace TauStellwerk.CommandStations.ECoS;

public class ECoSCommandStation : CommandStationBase
{
    private readonly ILogger<CommandStationBase> _logger;
    private readonly ECosConnectionHandler _connectionHandler;

    public ECoSCommandStation(ECoSOptions options, ILogger<CommandStationBase> logger)
        : base()
    {
        _logger = logger;
        var ipAddress = options.IPAddress ?? throw new FormatException("ECoS has been configured as CommandStation, but no IPAddress was configured.");
        var port = options.Port;
        _connectionHandler = new ECosConnectionHandler(IPAddress.Parse(ipAddress), port, logger);

        _ = _connectionHandler.RegisterEvent("request(1,view)", "1", HandleStatusEvent);
    }

    /// <inheritdoc/>
    public override async Task LoadEnginesFromSystem(StwDbContext context)
    {
        try
        {
            var result = await _connectionHandler.SendCommandAsync("queryObjects(10,name,protocol)");
            var ecosEngines = ECoSMessageDecoder.DecodeEngineListMessage(result.Value.Content);

            foreach (var (id, name, protocol) in ecosEngines)
            {
                var engine = context.Engines.Include(e => e.ECoSEngineData)
                    .Include(e => e.Functions)
                    .SingleOrDefault(e => e.ECoSEngineData != null && e.ECoSEngineData.Id == id);

                // No support for consists yet, so ignore all engines with Protocol "MULTI".
                if (protocol == "MULTI")
                {
                    continue;
                }

                if (engine == null)
                {
                    engine = new Engine
                    {
                        Name = name,
                        ECoSEngineData = new ECoSEngineData
                        {
                            Id = id,
                            Name = name,
                        },
                    };

                    _logger.LogInformation("Loaded new engine from ECoS: {engine}", engine);
                    await context.Engines.AddAsync(engine);
                }

                var loadedFunctions = (await GetEngineFunctionsFromECoS(engine)).ToList();

                engine.Functions.AddRange(loadedFunctions.Where(fl =>
                    !engine.Functions.Exists(f =>
                        f.Number == fl.Number)));

                await context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public override async Task HandleSystemStatus(State state)
    {
        await _connectionHandler.SendCommandAsync($"set(1,{(state == State.On ? "go" : "stop")})");
    }

    public override async Task HandleEngineSpeed(Engine engine, short speed, Direction priorDirection, Direction newDirection)
    {
        var ecosData = CheckForEcosData(engine);
        var directionTask = priorDirection != newDirection
            ? _connectionHandler.SendCommandAsync($"set({ecosData.Id},dir[{(newDirection == Direction.Forwards ? "0" : "1")}])")
            : Task.CompletedTask;

        var speedTask = _connectionHandler.SendCommandAsync($"set({ecosData.Id},speed[{speed}])");

        await directionTask;
        await speedTask;
    }

    public override async Task HandleEngineEStop(Engine engine, Direction priorDirection)
    {
        var ecosData = CheckForEcosData(engine);
        await _connectionHandler.SendCommandAsync($"set({ecosData.Id},stop)");
    }

    /// <inheritdoc/>
    public override async Task HandleEngineFunction(Engine engine, byte functionNumber, State state)
    {
        var engineData = CheckForEcosData(engine);
        await _connectionHandler.SendCommandAsync($"set({engineData.Id},func[{functionNumber},{(state == State.On ? "1" : "0")}])");
    }

    public override async Task CheckState()
    {
        var statusMessage = await _connectionHandler.SendCommandAsync("get(1,status)");
        HandleStatusEvent(statusMessage.Value);
    }

    private static ECoSEngineData CheckForEcosData(Engine engine)
    {
        if (engine.ECoSEngineData == null)
        {
            throw new ArgumentException("ECoS-CommandStation cannot handle engines without ECoS-Data");
        }

        return engine.ECoSEngineData;
    }

    private void HandleStatusEvent(ECoSMessage message)
    {
        if (message.Content.Contains("status[GO]"))
        {
            OnStatusChange(State.On);
            return;
        }

        if (message.Content.Contains("status[STOP]"))
        {
            OnStatusChange(State.Off);
        }
    }

    private async Task<IList<DccFunction>> GetEngineFunctionsFromECoS(Engine engine)
    {
        var ecosData = CheckForEcosData(engine);

        var response = await _connectionHandler.SendCommandAsync($"get({ecosData.Id},funcdesc)");
        var functions = ECoSMessageDecoder.DecodeFuncdescMessage(response.Value.Content);

        var dccFunctions = new List<DccFunction>();

        foreach (var (number, type, _) in functions)
        {
            if (type != 0)
            {
                dccFunctions.Add(new DccFunction(number, string.Empty, -1));
            }
        }

        return dccFunctions;
    }
}
