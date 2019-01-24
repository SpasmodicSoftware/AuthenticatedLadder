using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticatedLadder.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace AuthenticatedLadder.Persistence
{
    public class LadderDBContext : DbContext
    {
        public LadderDBContext(DbContextOptions<LadderDBContext> options) : base(options)
        {

        }

        public DbSet<LadderEntry> Ladders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LadderEntry>()
                .HasKey(l => new { l.LadderId, l.Platform, l.Username });

            //modelBuilder.Entity<LadderEntry>()
            //    .Property<DateTime>("EntryDate")
            //    .ValueGeneratedOnAddOrUpdate();
        }
    }
}
