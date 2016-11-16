using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaD.Models
{
    public class ReportedProject : ReportedEntityBase
    {
        public int ReportedProjectId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}