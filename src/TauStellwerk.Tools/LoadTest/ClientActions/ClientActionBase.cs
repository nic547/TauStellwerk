﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Client.Services;

namespace TauStellwerk.Tools.LoadTest.ClientActions;

/// <summary>
/// A action performed by a client.
/// </summary>
public abstract class ClientActionBase
{
    private EngineService? _engineService;
    private LoadTestOptions? _options;
    private Random? _random;

    /// <summary>
    /// Gets the interval in which the actions should be performed.
    /// </summary>
    public abstract int Interval { get; }

    /// <summary>
    /// Gets the HttpClient to use.
    /// </summary>
    internal EngineService EngineService
    {
        get => _engineService ?? throw new InvalidOperationException();
        private set => _engineService = value;
    }

    /// <summary>
    /// Gets the Options to use.
    /// </summary>
    internal LoadTestOptions Options
    {
        get => _options ?? throw new InvalidOperationException();
        private set => _options = value;
    }

    /// <summary>
    /// Gets an instance of <see cref="Random"/> to use.
    /// </summary>
    internal Random Random
    {
        get => _random ?? throw new InvalidOperationException();
        private set => _random = value;
    }

    internal int Id { get; set; }

    public virtual Task Initialize(EngineService engineService, LoadTestOptions options, int id, Random random)
    {
        EngineService = engineService;
        Options = options;
        Random = random;
        Id = id;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Perform the action, send the http request to the server.
    /// </summary>
    /// <returns>Time until response was received.</returns>
    public abstract Task<int> PerformRequest();
}
