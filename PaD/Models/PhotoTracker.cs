using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PaD.Models
{
    public class PhotoTracker
    {
        public int PhotoTrackerId { get; set; }

        [Required]
        [Index("IX_PhotoId", 1, IsUnique = true)]
        public int PhotoId { get; set; }

        [Required]
        public long Counter { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public PhotoTracker() { }
    }
}