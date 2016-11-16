using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using PaD.Models;

namespace PaD.DataContexts
{
    public interface IPaDDb : IDbContext
    {
        DbSet<Project> Project { get; set; }
        DbSet<Photo> Photo { get; set; }
        DbSet<PhotoImage> PhotoImage { get; set; }
        DbSet<ThumbnailImage> ThumbnailImage { get; set; }
        DbSet<PhotoRating> PhotoRating { get; set; }
        DbSet<FeaturedProject> FeaturedProject { get; set; }
        DbSet<ProjectTracker> ProjectTracker { get; set; }
        DbSet<PhotoTracker> PhotoTracker { get; set; }
        DbSet<ReportedProject> ReportedProject { get; set; }
        DbSet<ReportedPhoto> ReportedPhoto { get; set; }

    }
}