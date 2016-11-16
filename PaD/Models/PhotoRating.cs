using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaD.Models
{
    public partial class PhotoRating
    {
        public int PhotoRatingId { get; set; }

        [Required]
        [StringLength(256)]
        [Index("IX_IdentityUserIdAndPhotoId", 1, IsUnique = true)]
        public string IdentityUserName { get; set; }

        [Required]
        [Index("IX_IdentityUserIdAndPhotoId", 2, IsUnique = true)]
        public int PhotoId { get; set; }

        [Required]
        public double Value { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        //public virtual IdentityUser IdentityUser { get; set; }

        public virtual Photo Photo { get; set; }
    }
}
