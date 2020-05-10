using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PiStellwerk.Data
{
    public class StwDbContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=StwDatabase.db");
        }

        public DbSet<Engine> Engines { get; set; }
    }
}
 