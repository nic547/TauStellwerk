// <copyright file="EngineControllerTestsBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PiStellwerk.Base.Model;
using PiStellwerk.Controllers;
using PiStellwerk.Database;
using PiStellwerk.Database.Model;
using PiStellwerk.Services;

namespace PiStellwerk.Test.ControllerTests.EngineControllerTests
{
    public class EngineControllerTestsBase
    {
        protected int EngineId { get; set; }

        protected string ConnectionString { get; set; } = string.Empty;

        protected SqliteConnection? Connection { get; set; }

        protected string SessionId { get; set; } = string.Empty;

        protected SessionService? Service { get; set; }

        /// <summary>
        /// Does the setup for the tests. Sets up a in-memory sqlite database etc.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var rnd = new Random();
            ConnectionString = $"Data Source={rnd.Next()};Mode=Memory;Cache=Shared";

            // SQLite removes a database when the connection is closed. By keeping a connection open until teardown, we can prevent this from happening.
            Connection = new SqliteConnection(ConnectionString);
            Connection.Open();

            var context = GetContext();
            context.Database.EnsureCreated();

            var testEngine = GetTestEngine();
            context.Engines.Add(testEngine);
            context.SaveChanges();

            EngineId = testEngine.Id;

            Service = new SessionService();
            SessionId = Service.CreateSession("tesUser", "io9urjhgf9rh").SessionId;
        }

        /// <summary>
        /// Closes the Database connection used to keep the SQLite db "in-memory".
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Connection?.Close();
        }

        protected EngineController GetController()
        {
            return GetController(GetMock(true));
        }

        protected EngineController GetController(IEngineService engineService)
        {
            return new(GetContext(), engineService, Service!);
        }

        protected StwDbContext GetContext()
        {
            var contextOptions = new DbContextOptionsBuilder<StwDbContext>().UseSqlite(ConnectionString);
            return new StwDbContext(contextOptions.Options);
        }

        protected Engine GetTestEngine()
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
                Images =
                {
                    new EngineImage(),
                    new EngineImage(),
                },
                ECoSEngineData = new ECoSEngineData
                {
                    Id = 1001,
                    Name = "Nightpiercer",
                },
            };
        }

        protected IEngineService GetMock(bool returns)
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