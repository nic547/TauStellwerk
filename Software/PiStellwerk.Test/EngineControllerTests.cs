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
using PiStellwerk.Data.Commands;

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
        }

        /// <summary>
        /// Closes the Database connection used to keep the SQLite db "in-memory".
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        /// <summary>
        /// Ensure that an engine can receive commands after being acquired.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CanSetSpeedOfAcquiredEngine()
        {
            _controller.AcquireEngine(_engineId);
            var result = await _controller.SetEngineSpeed(_engineId, 100, null);

            Assert.IsInstanceOf(typeof(OkResult), result);
        }

        /// <summary>
        /// Ensure that an engine cannot receive commands when not acquired.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CannotSetSpeedOfUnacquiredEngine()
        {
            JsonCommand jsonCommand = new()
            {
                Type = CommandType.Speed,
                Data = 100,
            };
            var result = await _controller.SetEngineSpeed(_engineId, 23, true) as ObjectResult;

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
            JsonCommand jsonCommand = new()
            {
                Type = CommandType.Speed,
                Data = 100,
            };

            _controller.AcquireEngine(_engineId);
            _controller.ReleaseEngine(_engineId);

            var result = await _controller.SetEngineSpeed(_engineId, 124, null) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        /// <summary>
        /// Ensure an engine cannot be acquired twice.
        /// </summary>
        [Test]
        public void EngineCannotBeAcquiredTwice()
        {
            _controller.AcquireEngine(_engineId);
            var result = _controller.AcquireEngine(_engineId) as ObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status423Locked, result.StatusCode);
        }

        /// <summary>
        /// Ensure trying to acquire a non-existent engine does return a 404.
        /// </summary>
        [Test]
        public void NotExistingEngineCannotBeAcquired()
        {
            var result = _controller.AcquireEngine(_engineId + 1337) as ObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}
