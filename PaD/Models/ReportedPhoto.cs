using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PaD.Models
{
    [Table("ReportedPhotos")]
    public class ReportedPhoto : ReportedEntityBase
    {
        public int ReportedPhotoId { get; set; }

        [Required]
        public int PhotoId { get; set; }

        public virtual Photo Photo { get; set; }
    }
}