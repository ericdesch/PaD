using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaD.Models
{
    public class ReportedUser : ReportedEntityBase
    {
        public int ReportedUserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }
    }
}