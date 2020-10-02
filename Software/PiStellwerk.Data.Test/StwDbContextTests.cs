// <copyright file="StwDbContextTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;

namespace PiStellwerk.Data.Test
{
    /// <summary>
    /// Contains tests related to the DbContext of the application.
    /// </summary>
    public class StwDbContextTests
    {
        private StwDbContext _context;

        /// <summary>
        /// Does the setup for tests. Creates a SQLite in-memory database.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _context = new StwDbContext("Data Source=:memory:");
            _context.Database.EnsureCreated();
        }

        /// <summary>
        /// Empty test to check if the setup can run.
        /// </summary>
        [Test]
        public void RunSetupTest()
        {
            Assert.Pass();
        }
    }
}