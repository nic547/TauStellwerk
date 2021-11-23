// <copyright file="SessionService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TauStellwerk.Base.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Services;

/// <summary>
/// Service that keeps track of users.
/// </summary>
public class SessionService : BackgroundService
{
    private const int TimeoutInactive = 60;
    private const int TimeoutDeletion = 3600;

    private readonly ConcurrentDictionary<string, Session> _sessions = new();
    private readonly INowProvider _now;

    public SessionService(INowProvider? now = null)
    {
        _now = now ?? new NowProvider();
    }

    public delegate void SessionTimeoutHandler(Session session);

    public event SessionTimeoutHandler? SessionTimeout;

    public Session CreateSession(string username, string? userAgent, string sessionId)
    {
        var session = new Session
        {
            UserAgent = userAgent ?? string.Empty,
            UserName = username,
            LastContact = _now.GetNow(),
            SessionId = sessionId,
        };
        _sessions.TryAdd(session.SessionId, session);
        ConsoleService.PrintMessage($"{session} created new session");
        return session;
    }

    public bool TryUpdateSessionLastContact(string sessionId)
    {
        var session = TryGetSession(sessionId);
        if (session == null)
        {
            Console.WriteLine($"Update for non-existant session:{sessionId}");
            return false;
        }

        session.LastContact = _now.GetNow();
        return true;
    }

    public void RenameSessionUser(string sessionId, string newUsername)
    {
        var session = TryGetSession(sessionId);
        if (session == null)
        {
            throw new ArgumentException($"No user with user id {sessionId} found. Requested username: {newUsername}");
        }

        ConsoleService.PrintMessage($"{session} renamed to {newUsername}");
        session.UserName = newUsername;
    }

    public Session? TryGetSession(string sessionId)
    {
        _sessions.TryGetValue(sessionId, out var value);
        return value;
    }

    /// <summary>
    /// Get a list of all active users.
    /// </summary>
    /// <returns>The list of active users.</returns>
    public IReadOnlyList<Session> GetSessions()
    {
        return _sessions.Values.ToList();
    }

    public void CheckLastContacts()
    {
        foreach (var session in _sessions.Values)
        {
            var idle = (_now.GetNow() - session.LastContact).TotalSeconds;
            switch (idle)
            {
                case > TimeoutInactive when session.IsActive:
                    session.IsActive = false;
                    SessionTimeout?.Invoke(session);
                    ConsoleService.PrintMessage($"{session} has been marked as inactive.");
                    break;

                case < TimeoutInactive when !session.IsActive:
                    session.IsActive = true;
                    ConsoleService.PrintMessage($"{session} has been reactivated.");
                    break;

                case > TimeoutDeletion:
                    _sessions.TryRemove(session.SessionId, out _);
                    ConsoleService.PrintMessage($"{session} has been deleted after {Math.Round(idle)} seconds");
                    break;
            }
        }
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConsoleService.PrintMessage("SessionService is starting.");

        stoppingToken.Register(() => Console.WriteLine("SessionService is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            CheckLastContacts();

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        ConsoleService.PrintMessage("SessionService background task is stopping.");
    }
}