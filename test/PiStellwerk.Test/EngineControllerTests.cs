// <copyright file="EngineControllerTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PiStellwerk.Commands;
using PiStellwerk.Controllers;
using PiStellwerk.Data;
using PiStellwerk.Database;
using PiStellwerk.Services;

namespace PiStellwerk.Test
{
    /// <summary>
    /// Tests related to <see cref="EngineController"/>.
    /// </summary>
    public class EngineControllerTests
    {
        private int _engineId;

        private string _connectionString = string.Empty;
        private SqliteConnection _connection;
        private string _sessionId = string.Empty;

        private EngineService _engineService;

        /// <summary>
        /// Does the setup for the tests. Sets up a in-memory sqlite database etc.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var rnd = new Random();
            _connectionString = $"Data Source={rnd.Next()};Mode=Memory;Cache=Shared";

            // SQLite removes a database when the connection is closed. By keeping a connection open until teardown, we can prevent this from happening.
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();

            var context = new StwDbContext(_connectionString);
            context.Database.EnsureCreated();

            var testEngine = GetTestEngine();
            context.Engines.Add(testEngine);
            context.SaveChanges();

            _engineId = testEngine.Id;

            _engineService = new EngineService(new NullCommandSystem());

            var sessionController = new SessionController();
            if (sessionController.CreateSession("testUser", "none") is ObjectResult sessionResult)
            {
                _sessionId = sessionResult.Value.ToString();
            }
        }

        /// <summary>
        /// Closes the Database connection used to keep the SQLite db "in-memory".
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _connection.Close();
            SessionService.CleanSessions();
        }

        [Test]
        public async Task CanLoadList()
        {
            var list = await GetController().GetEngines();
            list.Should().NotBeEmpty();
        }

        /// <summary>
        /// Ensure that the engine list only includes necessary properties.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task LoadedListOnlyIncludesSomeProperties()
        {
            var list = await GetController().GetEngines();
            list[0].Functions.Should().BeEmpty();
            list[0].ECoSEngineData.Should().BeNull();
            list[0].Image.Should().NotBeEmpty();
        }

        [Test]
        public async Task CanAddEngine()
        {
            var engineToAdd = new Engine
            {
                Address = 392,
                Name = "Re 620 088-5 (xrail)",
                TopSpeed = 140,
                Tags = new List<string>
                {
                    "Freight",
                },
                Functions = new List<DccFunction>()
                {
                    new(0, "Headlights"),
                },
            };

            var returnedEngine = (await GetController().UpdateOrAdd(engineToAdd)).Value;
            var loadedEngine = await GetController().GetEngine(returnedEngine.Id);
            var list = await GetController().GetEngines();

            engineToAdd.Id = returnedEngine.Id;
            engineToAdd.Should().BeEquivalentTo(returnedEngine);
            returnedEngine.Should().BeEquivalentTo(loadedEngine);
            list.Should().HaveCount(2);
        }

        [Test]
        public async Task CannotAddEngineWithId()
        {
            var engine = new Engine
            {
                Id = int.MaxValue,
            };

            var result = await GetController().UpdateOrAdd(engine);
            result.Result.Should().BeAssignableTo<UnprocessableEntityResult>();
        }

        [Test]
        public async Task UpdatingDoesntRemoveECoSData()
        {
            var engine = await GetController().GetEngine(1);
            engine.ECoSEngineData.Should().BeNull();
            await GetController().UpdateOrAdd(engine);

            var updatedEngine = GetContext().Engines
                .Include(e => e.ECoSEngineData)
                .Single(e => e.Id == 1);

            updatedEngine.ECoSEngineData.Should().NotBeNull();
        }

        [Test]
        public async Task CanDeleteEngine()
        {
            var result = await GetController().Delete(1);
            var list = await GetController().GetEngines();

            result.Should().BeAssignableTo<OkResult>();
            list.Should().BeEmpty();
        }

        [Test]
        public async Task CannotDeleteNonExistentEngine()
        {
            var result = await GetController().Delete(int.MaxValue);
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Test]
        public async Task SpeedFailureReturnsBadRequest()
        {
            var result = await GetController(GetMock(false)).SetEngineSpeed(_sessionId, _engineId, 100, null);
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Test]
        public async Task SpeedInvalidSessionReturns403()
        {
            var result = await GetController().SetEngineSpeed("InvalidSession", 1, 120, false);
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(403);
        }

        [Test]
        public async Task SpeedSuccessCase()
        {
            var result = await GetController(GetMock(true)).SetEngineSpeed(_sessionId, 1, 80, false);
            result.Should().BeAssignableTo<OkResult>();
        }

        [Test]
        public async Task FunctionInvalidSessionReturns403()
        {
            var result = await GetController().EngineFunction("invalidSession", 1, 1, "on");
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(403);
        }

        [Test]
        public async Task FunctionFailureReturnsBadRequest()
        {
            var result = await GetController(GetMock(false)).EngineFunction(_sessionId, 1, 1, "on");
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Test]
        public async Task FunctionSuccessCase()
        {
            var result = await GetController(GetMock(true)).EngineFunction(_sessionId, 1, 1, "on");
            result.Should().BeAssignableTo<OkResult>();
        }

        [Test]
        public async Task AcquireInvalidSessionReturns403()
        {
            var result = await GetController().AcquireEngine(1, "InvalidSession");
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(403);
        }

        [Test]
        public async Task AcquireNonExistentEngineReturnsNotFound()
        {
            var result = await GetController().AcquireEngine(int.MaxValue, _sessionId);
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Test]
        public async Task AcquireFailureReturnsLocked()
        {
            var result = await GetController(GetMock(false)).AcquireEngine(1, _sessionId);
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Which.StatusCode.Should().Be(423);
        }

        [Test]
        public async Task AcquireSuccessCase()
        {
            var result = await GetController(GetMock(true)).AcquireEngine(1, _sessionId);
            result.Should().BeAssignableTo<OkResult>();
        }

        [Test]
        public async Task ReleaseInvalidSessionReturns403()
        {
            var result = await GetController().ReleaseEngine(1, "InvalidSession");
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(403);
        }

        [Test]
        public async Task ReleaseFailureReturnsBadRequest()
        {
            var result = await GetController(GetMock(false)).ReleaseEngine(1, _sessionId);
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Test]
        public async Task RelasaeSuccessCase()
        {
            var result = await GetController(GetMock(true)).ReleaseEngine(1, _sessionId);
            result.Should().BeAssignableTo<OkResult>();
        }

        private EngineController GetController()
        {
            return GetController(_engineService);
        }

        private EngineController GetController(IEngineService engineService)
        {
            return new(GetContext(), engineService);
        }

        private StwDbContext GetContext()
        {
            return new(_connectionString);
        }

        private Engine GetTestEngine()
        {
            return new()
            {
                Address = 492,
                Name = "Hupac Vectron (Nightpiercer)",
                SpeedSteps = 128,
                Functions = new List<DccFunction>
                {
                    new(0, "Headlights"),
                    new(1, "Sound"),
                    new(2, "Horn, high - long"),
                    new(3, "Horn, low - long"),
                    new(4, "Compressor on/off"),
                    new(5, "Connect/Disconnect Coupling"),
                    new(6, "Shunting light and gear"),
                    new(7, "High beam"),
                },
                Image =
                {
                    new EngineImage(string.Empty),
                    new EngineImage(string.Empty),
                },
                ECoSEngineData = new ECoSEngineData
                {
                    Id = 1001,
                    Name = "Nightpiercer",
                },
            };
        }

        private IEngineService GetMock(bool returns)
        {
            var mock = new Mock<IEngineService>();
            mock.Setup(e => e.AcquireEngine(
                    It.IsAny<Session>(),
                    It.IsAny<Engine>()))
                .ReturnsAsync(returns);
            mock.Setup(e => e.ReleaseEngine(
                    It.IsAny<Session>(),
                    It.IsAny<int>()))
                .ReturnsAsync(returns);
            mock.Setup(e => e.SetEngineFunction(
                    It.IsAny<Session>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(returns);
            mock.Setup(e => e.SetEngineSpeed(
                    It.IsAny<Session>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool?>()))
                .ReturnsAsync(returns);
            return mock.Object;
        }
    }
}
