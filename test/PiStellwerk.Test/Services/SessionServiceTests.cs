// <copyright file="SessionServiceTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using FluentAssertions;
using NUnit.Framework;
using PiStellwerk.Services;
using PiStellwerk.Util;

namespace PiStellwerk.Test.Services
{
    public class SessionServiceTests
    {
        [Test]
        public void SessionWithRegularUpdatesIsNotInactive()
        {
            var nowProvider = new StaticTimeProvider();
            var sessionService = new SessionService(nowProvider);
            var session = sessionService.CreateSession("TESTUSER", "testthing");

            for (var i = 0; i < 10; i++)
            {
                nowProvider.DateTime += TimeSpan.FromSeconds(15);
                sessionService.CheckLastContacts();
                sessionService.TryUpdateSessionLastContact(session.SessionId);
            }

            session.IsActive.Should().BeTrue();
        }

        [Test]
        public void SessionWithoutUpdateIsInactive()
        {
            var nowProvider = new StaticTimeProvider();
            var sessionService = new SessionService(nowProvider);
            var session = sessionService.CreateSession("TESTUSER", "testthing");

            nowProvider.DateTime += TimeSpan.FromSeconds(100);
            sessionService.CheckLastContacts();

            session.IsActive.Should().BeFalse();
        }

        [Test]
        public void SessionCanBeReactivated()
        {
            var nowProvider = new StaticTimeProvider();
            var sessionService = new SessionService(nowProvider);
            var session = sessionService.CreateSession("TESTUSER", "testthing");

            nowProvider.DateTime += TimeSpan.FromSeconds(100);
            sessionService.CheckLastContacts();

            session.IsActive.Should().BeFalse();

            sessionService.TryUpdateSessionLastContact(session.SessionId);
            sessionService.CheckLastContacts();

            session.IsActive.Should().BeTrue();
        }

        [Test]
        public void OldSessionsGetRemoved()
        {
            var nowProvider = new StaticTimeProvider();
            var sessionService = new SessionService(nowProvider);
            var session = sessionService.CreateSession("TESTUSER", "testthing");

            nowProvider.DateTime += TimeSpan.FromHours(2);
            sessionService.CheckLastContacts();
            sessionService.CheckLastContacts();

            sessionService.TryGetSession(session.SessionId).Should().BeNull();
        }

        private class StaticTimeProvider : INowProvider
        {
            public DateTime DateTime { get; set; } = DateTime.Now;

            public DateTime GetNow()
            {
                return DateTime;
            }
        }
    }
}
