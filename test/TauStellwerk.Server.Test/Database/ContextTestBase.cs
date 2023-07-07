// <copyright file="ContextTestBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TauStellwerk.Server.Data;

namespace TauStellwerk.Test.Database;

public class ContextTestBase
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

    internal StwDbContext GetContext()
    {
        var contextOptions = new DbContextOptionsBuilder<StwDbContext>().UseSqlite(_connectionString);
        return new StwDbContext(contextOptions.Options);
    }
}
