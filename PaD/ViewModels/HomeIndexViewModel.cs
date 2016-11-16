using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PaD.Models;

namespace PaD.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<PhotoViewModel> TopPhotos { get; set; }
        public FeaturedProjectViewModel FeaturedProject { get; set; }
        public string CurrentStreak { get; set; }
    }
}