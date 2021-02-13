﻿// <copyright file="SessionService.cs" company="Dominic Ritz">
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
using PiStellwerk.Data;

namespace PiStellwerk.BackgroundServices
{
    /// <summary>
    /// Service that keeps track of users.
    /// </summary>
    public class SessionService : BackgroundService
    {
        private const int _timeoutWarning = 10;
        private const int _timeoutRemoval = 30;
        private static readonly ConcurrentDictionary<string, Session> _sessions = new();

        public static Session CreateSession(string username, string userAgent)
        {
            var session = new Session
            {
                UserAgent = userAgent,
                UserName = username,
                LastContact = DateTime.Now,
            };
            _sessions.TryAdd(session.SessionId, session);
            return session;
        }

        public static void UpdateSessionLastContact(string sessionId)
        {
            var session = TryGetSession(sessionId);
            if (session == null)
            {
                throw new ArgumentException($"Reported contact with sessionId {sessionId}, but no such session was found.");
            }

            session.LastContact = DateTime.Now;
        }

        public static void RenameSessionUser(string sessionId, string newUsername)
        {
            var session = TryGetSession(sessionId);
            if (session == null)
            {
                throw new ArgumentException($"No user with user id {sessionId} found. Requested username: {newUsername}");
            }

            Console.WriteLine($"Renaming {session.UserName} to {newUsername}");
            session.UserName = newUsername;
        }

        public static Session? TryGetSession(string sessionId)
        {
            _sessions.TryGetValue(sessionId, out var value);
            return value;
        }

        /// <summary>
        /// Get a list of all active users.
        /// </summary>
        /// <returns>The list of active users.</returns>
        public static IReadOnlyList<Session> GetSessions()
        {
            return _sessions.Values.ToList();
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("SessionService is starting.");

            stoppingToken.Register(() => Console.WriteLine("SessionService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var session in _sessions.Values)
                {
                    var idle = (DateTime.Now - session.LastContact).TotalSeconds;
                    if (idle < _timeoutRemoval && idle > _timeoutWarning)
                    {
                        Console.WriteLine($"Session {session.UserName} has been inactive for {Math.Round(idle)} seconds");
                    }

                    if (idle > _timeoutRemoval)
                    {
                        _sessions.TryRemove(session.SessionId, out _);
                        Console.WriteLine($"Session {session.UserName} has been removed from the active session list");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Console.WriteLine("SessionService background task is stopping.");
        }
    }
}
