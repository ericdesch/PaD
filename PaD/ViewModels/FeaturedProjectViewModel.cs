using PaD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaD.ViewModels
{
    public class FeaturedProjectViewModel
    {
        public int ProjectId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string ThumbnailTitle { get; set; }
        public ImageViewModel ThumbnailImage { get; set; }
    }
}