// <copyright file="EngineControllerTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using PiStellwerk.Commands;
using PiStellwerk.Controllers;
using PiStellwerk.Data;
using PiStellwerk.Services;

namespace PiStellwerk.Test
{
    /// <summary>
    /// Tests related to <see cref="EngineController"/>.
    /// </summary>
    public class EngineControllerTests
    {
        private const int _engineId = 1;

        private EngineController _controller;
        private SqliteConnection _connection;
        private StwDbContext _context;
        private string _sessionId;
        private ICommandSystem _commandSystem;

        /// <summary>
        /// Does the setup for the tests. Sets up a in-memory sqlite database etc.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var rnd = new Random();
            var connectionString = $"Data Source={rnd.Next()};Mode=Memory;Cache=Shared";

            // SQLite removes a database when the connection is closed. By keeping a connection open until teardown, we can prevent this from happening.
            _connection = new SqliteConnection(connectionString);
            _connection.Open();

            _context = new StwDbContext(connectionString);
            _context.Database.EnsureCreated();

            _context.Engines.Add(new Engine
            {
                Address = 492,
                Name = "Hupac Nighpiercer",
                Id = _engineId,
                SpeedSteps = 128,
            });

            _context.SaveChanges();

            _commandSystem = new NullCommandSystem();

            _controller = new EngineController(_context, _commandSystem);

            var sessionController = new SessionController();
            if (sessionController.CreateSession("testuser", "none") is ObjectResult sessionResult)
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
            EngineController.ClearActiveEngines();
        }

        /// <summary>
        /// Ensure that an engine can receive commands after being acquired.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CanSetSpeedOfAcquiredEngine()
        {
            _controller.AcquireEngine(_engineId, _sessionId);
            var result = await _controller.SetEngineSpeed(_sessionId, _engineId, 100, null);

            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        /// <summary>
        /// Ensure that an engine cannot receive commands when not acquired.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CannotSetSpeedOfUnacquiredEngine()
        {
            var result = await _controller.SetEngineSpeed(_sessionId, _engineId, 23, true) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        /// <summary>
        /// Ensure that an engine cannot receive commands after being released.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CannotSetSpeedOfReleasedEngine()
        {
            _controller.AcquireEngine(_engineId, _sessionId);
            _controller.ReleaseEngine(_engineId, _sessionId);

            var result = await _controller.SetEngineSpeed(_sessionId, _engineId, 124, null) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Test]
        public async Task CannotSetSpeedWithWrongSession()
        {
            _controller.AcquireEngine(_engineId, _sessionId);

            var result = await _controller.SetEngineSpeed(_sessionId + "wrong", _engineId, 124, null) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
        }

        /// <summary>
        /// Ensure an engine cannot be acquired twice.
        /// </summary>
        [Test]
        public void EngineCannotBeAcquiredTwice()
        {
            _controller.AcquireEngine(_engineId, _sessionId);
            var result = _controller.AcquireEngine(_engineId, _sessionId) as ObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status423Locked, result.StatusCode);
        }

        /// <summary>
        /// Ensure trying to acquire a non-existent engine does return a 404.
        /// </summary>
        [Test]
        public void NotExistingEngineCannotBeAcquired()
        {
            var result = _controller.AcquireEngine(_engineId + 1337, _sessionId) as ObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        /// <summary>
        /// Ensure an invalid sessionId cannot be used to acquire an engine.
        /// </summary>
        [Test]
        public void InvalidSessionIdCannotAcquire()
        {
            var result = _controller.AcquireEngine(_engineId, "invalidId") as ObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
        }
    }
}
