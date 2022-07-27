// <copyright file="StwDbContext.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TauStellwerk.Server.Database.Model;

namespace TauStellwerk.Server.Database;

/// <summary>
/// <inheritdoc cref="DbContext"/>
/// </summary>
public class StwDbContext : DbContext
{
    public StwDbContext(DbContextOptions<StwDbContext> context)
        : base(context)
    {
    }

    /// <summary>
    /// Gets a collection of engines in the database.
    /// </summary>
    public DbSet<Engine> Engines => Set<Engine>();

    public DbSet<EngineImage> EngineImages => Set<EngineImage>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<Turnout> Turnouts => Set<Turnout>();

    internal class StwDbContextDesignTimeFactory : IDesignTimeDbContextFactory<StwDbContext>
    {
        public StwDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StwDbContext>();
            optionsBuilder.UseSqlite($"Filename=MigrationTestDatabase.db;cache=shared");

            return new StwDbContext(optionsBuilder.Options);
        }
    }
}