// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

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

    private static readonly char[] TagSeperators = ['\u001F'];

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<Engine>()
            .Property(e => e.Tags)
            .HasConversion(
                v => string.Join("\u001F", v),
                v => v.Split(TagSeperators, StringSplitOptions.RemoveEmptyEntries).ToList(),
                new ValueComparer<List<string>>(
                    (t1, t2) => t2 != null && t1 != null && t1.SequenceEqual(t2),
                    t => t.GetHashCode()));

        _ = modelBuilder.Entity<Engine>()
            .Property(e => e.ImageSizes)
            .HasConversion(
                list => string.Join("\u001F", list),
                savedValue => savedValue.Split(TagSeperators, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
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
            _ = optionsBuilder.UseSqlite($"Filename=MigrationTestDatabase.db;cache=shared");

            return new StwDbContext(optionsBuilder.Options);
        }
    }
}
