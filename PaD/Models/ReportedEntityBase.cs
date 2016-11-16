using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PaD.Models
{
    public enum ReportStatus
    {
        New,
        Pending,
        Closed
    }

    public class ReportedEntityBase
    {
        [Required]
        [MaxLength(256)]
        public string ReportedBy { get; set; }

        [Required]
        [DefaultValue(0)]
        public ReportStatus ReportStatus { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}