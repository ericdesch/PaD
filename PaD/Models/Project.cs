using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PaD.ViewModels;

namespace PaD.Models
{
    public class Project
    {
        public Project()
        {
            this.Photos = new HashSet<Photo>();
        }

        public int ProjectId { get; set; }

        [Required]
        [StringLength(256)]
        public string IdentityUserName { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        [DefaultValue("true")]
        public bool IsDefault { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }

        public Project(ProjectViewModel viewModel)
        {
            this.IdentityUserName = viewModel.UserName;
            this.IsDefault = viewModel.IsDefault;
            this.Title = viewModel.Title;
            this.ProjectId = viewModel.ProjectId;
            this.Photos = null;
        }
    }
}