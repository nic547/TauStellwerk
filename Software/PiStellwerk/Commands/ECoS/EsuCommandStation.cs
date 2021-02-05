// <copyright file="EsuCommandStation.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PiStellwerk.Data;
using PiStellwerk.Data.Commands;

#nullable enable

namespace PiStellwerk.Commands.ECoS
{
    /// <summary>
    /// Implementation of a <see cref="ICommandSystem"/> for the ESU Command Station (ECoS).
    /// </summary>
    public class EsuCommandStation : ICommandSystem
    {
        private readonly ECosConnectionHandler _connectionHandler;

        private bool _systemStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="EsuCommandStation"/> class.
        /// </summary>
        public EsuCommandStation()
        {
            // TODO: Remove hardcoded IP and Address
             _connectionHandler = new ECosConnectionHandler(IPAddress.Parse("192.168.1.153"), 15471);

             _ = _connectionHandler.RegisterEvent("request(1,view)", "1", HandleStatusEvent);
        }

        /// <inheritdoc/>
        public async Task LoadEnginesFromSystem(StwDbContext context)
        {
            try
            {
                var result = await _connectionHandler.SendCommandAsync("queryObjects(10, name, protocol)");
                var ecosEngines = ECoSMessageDecoder.DecodeEngineListMessage(result);

                foreach (var (id, name, protocol) in ecosEngines)
                {
                    Engine? engine = context.Engines.Include(e => e.ECoSEngineData)
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

                        await context.Engines.AddAsync(engine);
                    }

                    var loadedFunctions = (await GetEngineFunctionsFromECoS(engine!)).ToList();

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

        /// <inheritdoc/>
        public void HandleEngineCommand(JsonCommand command, Engine engine)
        {
             // DO nothing
        }

        /// <inheritdoc/>
        public async Task HandleEngineFunction(Engine engine, byte functionNumber, bool on)
        {
            var engineData = CheckForEcosData(engine);
            await _connectionHandler.SendCommandAsync($"set({engineData.Id},func[{functionNumber},{(on ? "1" : "0")}])");
        }

        /// <inheritdoc/>
        public Task<bool?> CheckStatusAsync()
        {
            return Task.FromResult<bool?>(_systemStatus);
        }

        /// <inheritdoc/>
        public bool TryAcquireEngine(Engine engine)
        {
            return true;
        }

        /// <inheritdoc/>
        public bool TryReleaseEngine(Engine engine)
        {
            return true;
        }

        private void HandleStatusEvent(string message)
        {
            _systemStatus = message.Contains("status[GO]");
        }

        private ECoSEngineData CheckForEcosData(Engine engine)
        {
            if (engine.ECoSEngineData == null)
            {
                throw new ArgumentException("ECoS-ICommandStation cannot handle engines without ECoS-Data");
            }

            return engine.ECoSEngineData;
        }

        private async Task<IList<DccFunction>> GetEngineFunctionsFromECoS(Engine engine)
        {
            var ecosData = CheckForEcosData(engine);

            var response = await _connectionHandler.SendCommandAsync($"get({ecosData.Id},funcdesc)");
            var functions = ECoSMessageDecoder.DecodeFuncdescMessage(response);

            var dccFunctions = new List<DccFunction>();

            foreach (var (number, type, isMomentary) in functions)
            {
                if (type != 0)
                {
                    dccFunctions.Add(new DccFunction(number, string.Empty));
                }
            }

            return dccFunctions;
        }
    }
}
