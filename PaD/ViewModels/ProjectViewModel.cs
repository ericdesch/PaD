using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaD.ViewModels
{
    public class ProjectViewModel
    {
        public int ProjectId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Title { get; set; }
        public bool IsDefault { get; set; }
        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }
        public ICollection<PhotoViewModel> PhotoViewModels { get; set; }

        public ProjectViewModel()
        {
            this.PhotoViewModels = new HashSet<PhotoViewModel>();
        }
    }
}