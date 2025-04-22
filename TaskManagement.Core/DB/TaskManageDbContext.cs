using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Model;
using static TaskManagement.Core.Model.Enums;

namespace TaskManagement.Core.DB
{
    public class TaskManageDbContext : DbContext
    {
        public TaskManageDbContext(DbContextOptions<TaskManageDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskObj> Tasks { get; set; }
        //public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.OwnerId).IsRequired();
                entity.HasIndex(e => e.OwnerId);
            });

            modelBuilder.Entity<TaskObj>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                // Configure value converter for Status
                entity.Property(e => e.Status)
                    .HasConversion(
                        v => v.ToString(), // Convert enum to string for the database
                        v => (StatusOfTask)Enum.Parse(typeof(StatusOfTask), v) // Convert string to enum for the application
                    )
                    .HasMaxLength(10);
                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ProjectId);
               // entity.HasIndex(e => e.AssignedToId);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
