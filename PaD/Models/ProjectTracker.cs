using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PaD.Models
{
    public class ProjectTracker
    {
        public int ProjectTrackerId { get; set; }

        [Required]
        [Index("IX_ProjectIdAndYearAndMonth", 1, IsUnique = true)]
        public int ProjectId { get; set; }

        [Required]
        [Index("IX_ProjectIdAndYearAndMonth", 2, IsUnique = true)]
        public int Month { get; set; }

        [Required]
        [Index("IX_ProjectIdAndYearAndMonth", 3, IsUnique = true)]
        public int Year { get; set; }

        [Required]
        public long Counter { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public ProjectTracker() { }

    }
}