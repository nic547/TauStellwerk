// <copyright file="StwDbContextTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace PiStellwerk.Data.Test
{
    /// <summary>
    /// Contains tests related to the DbContext of the application.
    /// </summary>
    public class StwDbContextTests
    {
        private string _connectionString;
        private SqliteConnection _sqliteConnection;
        private StwDbContext _context;

        /// <summary>
        /// Does the setup for tests. Creates a SQLite in-memory database.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var rnd = new Random();
            _connectionString = $"Data Source={rnd.Next()};Mode=Memory;Cache=Shared";

            // SQLite removes a database when the connection is closed. By keeping a connection open until teardown, we can prevent this from happening.
            _sqliteConnection = new SqliteConnection(_connectionString);
            _sqliteConnection.Open();

            _context = new StwDbContext(_connectionString);
            _context.Database.EnsureCreated();
        }

        /// <summary>
        /// Closes the Database connection used to keep the SQLite db "in-memory".
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _sqliteConnection.Close();
        }

        /// <summary>
        /// Test if a engine saved will be equal to the one loaded from it in a different dbContext.
        /// </summary>
        [Test]
        public void SaveAndLoadTest()
        {
            var originalEngine = TestDataHelper.CreateTestEngine();
            _context.Engines.Add(originalEngine);
            _context.SaveChanges();

            var loadContext = new StwDbContext(_connectionString);
            var loadedEngine = loadContext.Engines.Include(x => x.Functions).Single();

            loadedEngine.Should().NotBeSameAs(originalEngine);
            loadedEngine.Should().BeEquivalentTo(originalEngine);
        }

        /// <summary>
        /// Test that the testdata can be properly saved.
        /// </summary>
        [Test]
        public void InsertMultipleEngines()
        {
            var testData = TestDataService.GetEngines();
            _context.Engines.AddRange(testData);
            _context.SaveChanges();

            var testContext = new StwDbContext(_connectionString);
            var loadedEngines = testContext.Engines;

            Assert.AreEqual(testData.Count, loadedEngines.Count());
        }

        /// <summary>
        /// Test that updates to an engine get saved properly and we for example don't get a second engine inserted instead.
        /// </summary>
        [Test]
        public void UpdatePersistsTest()
        {
            var testFunction = new DccFunction(1, "Headlights");

            var originalEngine = TestDataHelper.CreateTestEngine();
            _context.Engines.Add(originalEngine);
            _context.SaveChanges();

            var updateContext = new StwDbContext(_connectionString);
            var updateEngine = updateContext.Engines.Include(x => x.Functions).Single();
            updateEngine.Functions = new List<DccFunction> { testFunction };
            updateContext.SaveChanges();

            var testContext = new StwDbContext(_connectionString);
            var testEngine = testContext.Engines.Include(x => x.Functions).Single();

            testEngine.Should().NotBeSameAs(updateEngine);
            testEngine.Should().BeEquivalentTo(updateEngine);
        }
    }
}