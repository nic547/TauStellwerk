// <copyright file="StwDbContext.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Data;

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

    public DbSet<Turnout> Turnouts => Set<Turnout>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Engine>()
            .Property(e => e.Tags)
            .HasConversion(
                v => string.Join("\u001F", v),
                v => v.Split(new char[] { '\u001F' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                new ValueComparer<List<string>>(
                    (t1, t2) => t2 != null && t1 != null && t1.SequenceEqual(t2),
                    t => t.GetHashCode()));

        modelBuilder.Entity<Engine>()
            .Property(e => e.ImageSizes)
            .HasConversion(
                list => string.Join("\u001F", list),
                savedValue => savedValue.Split(new[] { '\u001F' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
                new ValueComparer<List<int>>(
                    (t1, t2) => t2 != null && t1 != null && t1.SequenceEqual(t2),
                    t => t.GetHashCode()));

        base.OnModelCreating(modelBuilder);
    }

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