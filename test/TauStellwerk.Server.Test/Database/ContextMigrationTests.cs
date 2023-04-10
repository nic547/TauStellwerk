// <copyright file="ContextMigrationTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TauStellwerk.Server.Data;

namespace TauStellwerk.Test.Database;

public class ContextMigrationTests
{
    [Test]
    public void MigrationsCanBeApplied()
    {
        var rnd = new Random();
        var connectionString = $"Data Source={rnd.Next()};Mode=Memory;Cache=Shared";

        var dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        var contextOptions = new DbContextOptionsBuilder<StwDbContext>().UseSqlite(connectionString);
        var context = new StwDbContext(contextOptions.Options);
        Assert.DoesNotThrowAsync(async () => await context.Database.MigrateAsync());

        dbConnection.Close();
    }
}
