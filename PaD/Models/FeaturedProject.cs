using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PaD.ViewModels;

namespace PaD.Models
{
    public class FeaturedProject
    {
        public int FeaturedProjectId { get; set; }
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}