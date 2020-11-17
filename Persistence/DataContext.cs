﻿using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DbSet<Aircraft> Aircraft { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<AircraftCategory> AircraftCategory { get; set; }
        public DbSet<Type> Type { get; set; }
        public DbSet<AircraftType> AircraftType { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DataContext() : base() { }

        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many Aircraft <-> Category
            modelBuilder.Entity<AircraftCategory>()
                .HasKey(ac => new { ac.AircraftId, ac.CategoryId });

            modelBuilder.Entity<AircraftCategory>()
                .HasOne<Aircraft>(ac => ac.Aircraft)
                .WithMany(a => a.AircraftCategory)
                .HasForeignKey(ac => ac.AircraftId);

            modelBuilder.Entity<AircraftCategory>()
                .HasOne<Category>(ac => ac.Category)
                .WithMany(c => c.AircraftCategories)
                .HasForeignKey(ac => ac.CategoryId);

            // Many-to-many Aircraft <-> Type
            modelBuilder.Entity<AircraftType>()
                .HasKey(at => new { at.AircraftId, at.TypeId });

            modelBuilder.Entity<AircraftType>()
                .HasOne<Aircraft>(at => at.Aircraft)
                .WithMany(a => a.AircraftTypes)
                .HasForeignKey(at => at.AircraftId);

            modelBuilder.Entity<AircraftType>()
                .HasOne<Domain.Type>(at => at.Type)
                .WithMany(t => t.AircraftTypes)
                .HasForeignKey(at => at.TypeId);
        }
    }
}
