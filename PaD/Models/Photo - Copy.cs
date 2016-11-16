using PaD.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PaD.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }

        [Required]
        [Index("IX_ProjectIdAndDate", 1, IsUnique = true)]
        public int ProjectId { get; set; }

        [Required]
        [Column(TypeName = "Date")]
        [Index("IX_ProjectIdAndDate", 2, IsUnique = true)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(128)]
        public string Title { get; set; }

        [StringLength(128)]
        public string Alt { get; set; }

        [StringLength(128)]
        public string Tags { get; set; }

        [Required]
        [Display(Name="Photo of the Month")]
        public bool IsPhotoOfTheMonth { get; set; }

        [Required]
        [Display(Name="Photo of the Year")]
        public bool IsPhotoOfTheYear { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public virtual Project Project { get; set; }

        [Display(Name="Photo")]
        public virtual PhotoImage PhotoImage { get; set; }

        [Display(Name="Thumbnail")]
        public virtual ThumbnailImage ThumbnailImage { get; set; }

        public virtual ICollection<PhotoRating> PhotoRatings { get; set; }

        public Photo()
        {
            this.IsPhotoOfTheMonth = false;
            this.IsPhotoOfTheYear = false;
            this.PhotoRatings = new HashSet<PhotoRating>();
        }

        public Photo(PhotoViewModel photoViewModel)
        {
            this.PhotoId = photoViewModel.PhotoId;
            this.ProjectId = photoViewModel.ProjectId;
            this.Date = photoViewModel.Date;
            this.Title = photoViewModel.Title;
            this.Alt = photoViewModel.Alt;
            this.Tags = photoViewModel.Tags;
            this.IsPhotoOfTheMonth = photoViewModel.IsPhotoOfTheMonth;
            this.IsPhotoOfTheYear = photoViewModel.IsPhotoOfTheYear;

            // Add a new PhotoRatings object with the rating value, if supplied.
            if (photoViewModel.AuthenticatedUserRating > 0)
            {
                this.PhotoRatings = new List<PhotoRating>();
                this.PhotoRatings.Add(new PhotoRating()
                    {
                        IdentityUserName = photoViewModel.AuthenticatedUserName,
                        Value = photoViewModel.AuthenticatedUserRating
                    });
            }

            if (photoViewModel.PhotoImage != null)
            {
                if (this.PhotoImage == null)
                {
                    this.PhotoImage = new PhotoImage();
                }
                this.PhotoImage.Bytes = photoViewModel.PhotoImage.Bytes;
                this.PhotoImage.ContentType = photoViewModel.PhotoImage.ContentType;
            }

            if (photoViewModel.ThumbnailImage != null)
            {
                if (this.ThumbnailImage == null)
                {
                    this.ThumbnailImage = new ThumbnailImage();
                }
                this.ThumbnailImage.Bytes = photoViewModel.ThumbnailImage.Bytes;
                this.ThumbnailImage.ContentType = photoViewModel.ThumbnailImage.ContentType;
            }

            // If there are Photo bytes but no Thumbnail bytes, generate a thumbnail from Photo
            if (this.PhotoImage.Bytes != null && (this.ThumbnailImage == null || this.ThumbnailImage.Bytes == null))
            {
                this.GenerateThumbnail();
            }
        }
    }
}