// <copyright file="EngineServiceTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PiStellwerk.Commands;
using PiStellwerk.Data;
using PiStellwerk.Services;

namespace PiStellwerk.Test.Services
{
    public class EngineServiceTests
    {
        private readonly Session _session = new()
        {
            IsActive = true,
            LastContact = DateTime.Now,
            UserName = "Peter Müller",
            UserAgent = "---...---",
        };

        private readonly Engine _engine = new()
        {
            Id = 1,
        };

        [Test]
        public async Task CanAcquireEngineTest()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            var success = await service.AcquireEngine(_session, _engine);
            Assert.True(success);
        }

        [Test]
        public async Task CannotAcquireEngineTest()
        {
            var mock = GetAlwaysTrueMock();
            mock.Setup(e => e.TryAcquireEngine(It.IsAny<Engine>()).Result).Returns(false);
            var service = new EngineService(mock.Object);

            Assert.False(await service.AcquireEngine(_session, _engine));
        }

        [Test]
        public async Task CannotAcquireEngineTwice()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            Assert.True(await service.AcquireEngine(_session, _engine));
            Assert.False(await service.AcquireEngine(_session, _engine));
        }

        [Test]
        public async Task CanReleaseAcquiredEngine()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            Assert.True(await service.AcquireEngine(_session, _engine));
            Assert.True(await service.ReleaseEngine(_session, _engine.Id));
        }

        [Test]
        public async Task CannotReleaseEngineTwice()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            Assert.True(await service.AcquireEngine(_session, _engine));
            Assert.True(await service.ReleaseEngine(_session, _engine.Id));
            Assert.False(await service.ReleaseEngine(_session, _engine.Id));
        }

        [Test]
        public async Task DifferentSessionCannotRelease()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            var session2 = new Session
            {
                IsActive = true,
                LastContact = DateTime.Now,
                UserName = "Different Session",
            };

            Assert.True(await service.AcquireEngine(_session, _engine));
            Assert.False(await service.ReleaseEngine(session2, _engine.Id));
            Assert.True(await service.ReleaseEngine(_session, _engine.Id));
        }

        [Test]
        public async Task CanAcquireReleasedEngine()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            Assert.True(await service.AcquireEngine(_session, _engine));
            Assert.True(await service.ReleaseEngine(_session, _engine.Id));
            Assert.True(await service.AcquireEngine(_session, _engine));
        }

        [Test]
        public async Task CannotSetSpeedIfNotAcquired()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            Assert.False(await service.SetEngineSpeed(_session, 1, 100, null));
        }

        [Test]
        public async Task CanSetSpeedIfAcquired()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            await service.AcquireEngine(_session, _engine);
            Assert.True(await service.SetEngineSpeed(_session, 1, 100, null));
        }

        [Test]
        public async Task CannotSetSpeedWithInvalidSession()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);
            var session2 = new Session();

            await service.AcquireEngine(_session, _engine);
            Assert.False(await service.SetEngineSpeed(session2, 1, 100, null));
        }

        [Test]
        public async Task CannotSetFunctionIfNotAcquired()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            Assert.False(await service.SetEngineFunction(_session, 1, 2, false));
        }

        [Test]
        public async Task CanSetFunctionIfAcquired()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);

            await service.AcquireEngine(_session, _engine);
            Assert.True(await service.SetEngineFunction(_session, 1, 0, true));
        }

        [Test]
        public async Task CannotSetFunctionWithInvalidSession()
        {
            var mock = GetAlwaysTrueMock();
            var service = new EngineService(mock.Object);
            var session2 = new Session();

            await service.AcquireEngine(_session, _engine);
            Assert.False(await service.SetEngineFunction(session2, 1, 10, true));
        }

        private Mock<ICommandSystem> GetAlwaysTrueMock()
        {
            var mock = new Mock<ICommandSystem>();
            mock.Setup(e => e.TryAcquireEngine(It.IsAny<Engine>()).Result).Returns(true);
            mock.Setup(e => e.TryReleaseEngine(It.IsAny<Engine>()).Result).Returns(true);

            return mock;
        }
    }
}
