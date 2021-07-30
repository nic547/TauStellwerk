// <copyright file="EngineServiceTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PiStellwerk.Base.Model;
using PiStellwerk.Commands;
using PiStellwerk.Database.Model;
using PiStellwerk.Services;

namespace PiStellwerk.Test.Services
{
    public class EngineServiceTests
    {
        private readonly Engine _engine = new()
        {
            Id = 1,
        };

        [Test]
        public async Task CanAcquireEngineTest()
        {
            var (service, session) = PrepareEngineService();

            var success = await service.AcquireEngine(session, _engine);
            Assert.True(success);
        }

        [Test]
        public async Task CannotAcquireEngineTest()
        {
            var mock = GetAlwaysTrueMock();
            mock.Setup(e => e.TryAcquireEngine(It.IsAny<Engine>()).Result).Returns(false);
            var (service, session) = PrepareEngineService(mock);

            Assert.False(await service.AcquireEngine(session, _engine));
        }

        [Test]
        public async Task CannotAcquireEngineTwice()
        {
            var (service, session) = PrepareEngineService();

            Assert.True(await service.AcquireEngine(session, _engine));
            Assert.False(await service.AcquireEngine(session, _engine));
        }

        [Test]
        public async Task CanReleaseAcquiredEngine()
        {
            var (service, session) = PrepareEngineService();

            Assert.True(await service.AcquireEngine(session, _engine));
            Assert.True(await service.ReleaseEngine(session, _engine.Id));
        }

        [Test]
        public async Task CannotReleaseEngineTwice()
        {
            var (service, session) = PrepareEngineService();

            Assert.True(await service.AcquireEngine(session, _engine));
            Assert.True(await service.ReleaseEngine(session, _engine.Id));
            Assert.False(await service.ReleaseEngine(session, _engine.Id));
        }

        [Test]
        public async Task DifferentSessionCannotRelease()
        {
            var (service, session) = PrepareEngineService();

            var session2 = new Session
            {
                IsActive = true,
                LastContact = DateTime.Now,
                UserName = "Different Session",
            };

            Assert.True(await service.AcquireEngine(session, _engine));
            Assert.False(await service.ReleaseEngine(session2, _engine.Id));
            Assert.True(await service.ReleaseEngine(session, _engine.Id));
        }

        [Test]
        public async Task CanAcquireReleasedEngine()
        {
            var (service, session) = PrepareEngineService();

            Assert.True(await service.AcquireEngine(session, _engine));
            Assert.True(await service.ReleaseEngine(session, _engine.Id));
            Assert.True(await service.AcquireEngine(session, _engine));
        }

        [Test]
        public async Task CannotSetSpeedIfNotAcquired()
        {
            var (service, session) = PrepareEngineService();

            Assert.False(await service.SetEngineSpeed(session, 1, 100, null));
        }

        [Test]
        public async Task CanSetSpeedIfAcquired()
        {
            var (service, session) = PrepareEngineService();

            await service.AcquireEngine(session, _engine);
            Assert.True(await service.SetEngineSpeed(session, 1, 100, null));
        }

        [Test]
        public async Task CannotSetSpeedWithInvalidSession()
        {
            var (service, session) = PrepareEngineService();
            var session2 = new Session();

            await service.AcquireEngine(session, _engine);
            Assert.False(await service.SetEngineSpeed(session2, 1, 100, null));
        }

        [Test]
        public async Task CannotSetFunctionIfNotAcquired()
        {
            var (service, session) = PrepareEngineService();

            Assert.False(await service.SetEngineFunction(session, 1, 2, false));
        }

        [Test]
        public async Task CanSetFunctionIfAcquired()
        {
            var (service, session) = PrepareEngineService();

            await service.AcquireEngine(session, _engine);
            Assert.True(await service.SetEngineFunction(session, 1, 0, true));
        }

        [Test]
        public async Task CannotSetFunctionWithInvalidSession()
        {
            var (service, session) = PrepareEngineService();
            var session2 = new Session();

            await service.AcquireEngine(session, _engine);
            Assert.False(await service.SetEngineFunction(session2, 1, 10, true));
        }

        private (EngineService EngineService, Session Session) PrepareEngineService(Mock<CommandSystemBase>? mock = null)
        {
            var sessionService = new SessionService();
            var session = sessionService.CreateSession("TEST", "TEST");
            mock ??= GetAlwaysTrueMock();
            EngineService engineService = new(mock.Object, sessionService);
            return (engineService, session);
        }

        private Mock<CommandSystemBase> GetAlwaysTrueMock()
        {
            var configMock = new Mock<IConfiguration>();
            var mock = new Mock<CommandSystemBase>(configMock.Object);
            mock.Setup(e => e.TryAcquireEngine(It.IsAny<Engine>()).Result).Returns(true);
            mock.Setup(e => e.TryReleaseEngine(It.IsAny<Engine>()).Result).Returns(true);

            return mock;
        }
    }
}
