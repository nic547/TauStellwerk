// <copyright file="StwDbContextTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TauStellwerk.Database.Model;

namespace TauStellwerk.Database.Tests
{
    /// <summary>
    /// Contains tests related to the DbContext of the application.
    /// </summary>
    public class StwDbContextTests
    {
        private string _connectionString = string.Empty;
        private SqliteConnection? _sqliteConnection;

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

            var context = GetContext();
            context.Database.EnsureCreated();
        }

        /// <summary>
        /// Closes the Database connection used to keep the SQLite db "in-memory".
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            _sqliteConnection?.Close();
        }

        /// <summary>
        /// Test if a engine saved will be equal to the one loaded from it in a different dbContext.
        /// </summary>
        [Test]
        public void SaveAndLoadTest()
        {
            var originalEngine = TestDataHelper.CreateTestEngine();
            var context = GetContext();
            context.Engines.Add(originalEngine);
            context.SaveChanges();

            var loadContext = GetContext();
            var loadedEngine = loadContext.Engines
                .Include(x => x.Functions)
                .Include(e => e.Images)
                .Single();

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
            var context = GetContext();
            context.Engines.AddRange(testData);
            context.SaveChanges();

            var testContext = GetContext();
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
            var context = GetContext();
            context.Engines.Add(originalEngine);
            context.SaveChanges();

            var updateContext = GetContext();
            var updateEngine = updateContext.Engines.Include(x => x.Functions).Single();
            updateEngine.Functions.Clear();
            updateEngine.Functions.Add(testFunction);
            updateContext.SaveChanges();

            var testContext = GetContext();
            var testEngine = testContext.Engines.Include(x => x.Functions).Single();

            testEngine.Should().NotBeSameAs(updateEngine);
            testEngine.Should().BeEquivalentTo(updateEngine);
        }

        private StwDbContext GetContext()
        {
            var contextOptions = new DbContextOptionsBuilder<StwDbContext>().UseSqlite(_connectionString);
            return new StwDbContext(contextOptions.Options);
        }
    }
}