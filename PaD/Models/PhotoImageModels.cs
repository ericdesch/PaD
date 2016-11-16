using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PaD.Models
{
    // Would be nice to have these two classes inherit from a common base class since they are the same.
    // But EF gets upset/confused when you try to set up cascading deletes when you use inheritence.
    public interface IImage
    {
        byte[] Bytes { get; set; }
        string ContentType { get; set; }
    }

    public partial class PhotoImage : IImage
    {
        // 1 : 0..1 between Day and Photos so Key and ForeignKey are the same column.
        [Required]
        [Key, ForeignKey("Photo")]
        public int PhotoId { get; set; }

        [Required]
        public byte[] Bytes { get; set; }

        [Required]
        [StringLength(32)]
        public string ContentType { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public virtual Photo Photo { get; set; }
    }

    public class ThumbnailImage : IImage
    {
        // 1 : 0..1 between Day and Thumbnails so Key and ForeignKey are the same column.
        [Required]
        [Key, ForeignKey("Photo")]
        public int PhotoId { get; set; }

        [Required]
        public byte[] Bytes { get; set; }

        [Required]
        [StringLength(32)]
        public string ContentType { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public virtual Photo Photo { get; set; }
    }
}