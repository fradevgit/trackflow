using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrackFlow.Models.dbforall;

namespace TrackFlow.Data
{
    public partial class dbforallContext : DbContext
    {
        public dbforallContext()
        {
        }

        public dbforallContext(DbContextOptions<dbforallContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TrackFlow.Models.dbforall.AspNetUser>()
              .HasOne(i => i.Team)
              .WithMany(i => i.AspNetUsers)
              .HasForeignKey(i => i.TeamID)
              .HasPrincipalKey(i => i.TeamID);
    
            this.OnModelBuilding(builder);
                        builder.Entity<TrackFlow.Models.dbforall.Fine>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.Fines)
              .HasForeignKey(i => i.UserID)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<TrackFlow.Models.dbforall.Fine>()
              .HasOne(i => i.ViolationType1)
              .WithMany(i => i.Fines)
              .HasForeignKey(i => i.ViolationType)
              .HasPrincipalKey(i => i.ViolationID);

            builder.Entity<TrackFlow.Models.dbforall.ActivityRecord>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.ActivityRecords)
              .HasForeignKey(i => i.UserID)
              .HasPrincipalKey(i => i.Id);
            this.OnModelBuilding(builder);
        }

        public DbSet<TrackFlow.Models.dbforall.Team> Teams { get; set; }
        public DbSet<TrackFlow.Models.dbforall.Shift> Shifts { get; set; }
        public DbSet<TrackFlow.Models.dbforall.ViolationType> ViolationTypes { get; set; }

        public DbSet<TrackFlow.Models.dbforall.Fine> Fines { get; set; }

        public DbSet<TrackFlow.Models.dbforall.AspNetUser> AspNetUsers { get; set; }

        public DbSet<TrackFlow.Models.dbforall.ActivityRecord> ActivityRecords { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}