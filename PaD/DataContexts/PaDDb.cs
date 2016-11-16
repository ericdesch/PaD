using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using PaD.Models;

namespace PaD.DataContexts
{
    public class PaDDb : DbContext, IPaDDb
    {
        public PaDDb()
            : base("DefaultConnection")
        {
        }

        // Tell EF which entities to use
        public DbSet<Dictionary> Dictionary { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<PhotoImage> PhotoImage { get; set; }
        public DbSet<ThumbnailImage> ThumbnailImage { get; set; }
        public DbSet<PhotoRating> PhotoRating { get; set; }
        public DbSet<FeaturedProject> FeaturedProject { get; set; }

        public DbSet<ProjectTracker> ProjectTracker { get; set; }
        public DbSet<PhotoTracker> PhotoTracker { get; set; }

        public DbSet<ReportedProject> ReportedProject { get; set; }
        public DbSet<ReportedPhoto> ReportedPhoto { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure domain classes using modelBuilder here

            // Turn on cascading deletes for Photo.Photo (1:0..1)
            modelBuilder.Entity<Photo>()
                        .HasOptional(p => p.PhotoImage)
                        .WithRequired(t => t.Photo)
                        .WillCascadeOnDelete(true);

            // Turn on cascading deletes for Photo.Thumbnail (1:0..1)
            modelBuilder.Entity<Photo>()
                        .HasOptional(p => p.ThumbnailImage)
                        .WithRequired(i => i.Photo)
                        .WillCascadeOnDelete(true);

            // Cascading deletes on by default for for Photo.PhotoRatings (1:*)
            //modelBuilder.Entity<Photo>()
            //            .HasMany(p => p.PhotoRatings)
            //            .WithRequired(r => r.Photo)
            //            .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

    }
}