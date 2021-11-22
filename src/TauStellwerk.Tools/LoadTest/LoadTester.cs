// <copyright file="LoadTester.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using TauStellwerk.Tools.LoadTest.ClientActions;
using TauStellwerk.Util;

namespace TauStellwerk.Tools.LoadTest;

public static class LoadTester
{
    public static async Task Run(LoadTestOptions options)
    {
        var simulators = new List<ClientSimulator>();

        var actions = GetAvailableClientActions();

        for (var i = 0; i < options.Clients; i++)
        {
            var sim = await ClientSimulator.Create(actions, options, i + 1);
            sim.Start();
            simulators.Add(sim);
        }

        await Task.Delay(options.Time * 1000);

        var results = GatherResults(simulators);

        var max = results.Max(kv => kv.Key);
        foreach (var ms in Enumerable.Range(1, max))
        {
            Console.WriteLine($"{ms}ms : {results.GetByKey(ms)}");
        }

        Console.WriteLine($"Average: {results.Average()} ms");
        Console.WriteLine($"90th Percentile: {results.Get90ThPercentile()} ms");
        Console.WriteLine($"95th Percentile: {results.Get95ThPercentile()} ms");
        Console.WriteLine($"99th Percentile: {results.Get99ThPercentile()} ms");
    }

    private static CounterDictionary GatherResults(List<ClientSimulator> simulators)
    {
        var results = new CounterDictionary();

        foreach (var sim in simulators)
        {
            sim.Stop();
            results.Combine(sim.GetStatistics());
        }

        return results;
    }

    private static ImmutableList<Type> GetAvailableClientActions()
    {
        var actions = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ClientActionBase).IsAssignableFrom(p) && !p.IsAbstract)
            .ToImmutableList();
        return actions;
    }
}
