using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using PaD.Models;
using PaD.CustomFilters;

namespace PaD.ViewModels
{
    public class PhotoCreateViewModel : PhotoViewModel
    {
        // PostedFile is needed for Photos/Add and Photos/Edit. The uploaded photo
        // is retreived from this property. We also use this property to specify
        // the filesize limit in kB.
        // Required for Create, not for Edit.
        [FileSize(1024)]
        [Display(Name = "Photo")]
        [Required]
        public HttpPostedFileBase PostedFile { get; set; }

        public PhotoCreateViewModel() : base() { }
    }

    public class PhotoEditViewModel : PhotoViewModel
    {
        // PostedFile is needed for Photos/Add and Photos/Edit. The uploaded photo
        // is retreived from this property. We also use this property to specify
        // the filesize limit in kB.
        // Required for Create, not for Edit.
        [FileSize(1024)]
        [Display(Name = "Photo")]
        public HttpPostedFileBase PostedFile { get; set; }
        
        public PhotoEditViewModel() : base() { }
        public PhotoEditViewModel(PhotoViewModel photoViewModel) : base(photoViewModel) { }
    }

    public class PhotoViewModel //: IValidatableObject
    {
        public int PhotoId { get; set; }
        public int ProjectId { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }
        [Required]
        public string Title { get; set; }
        public string Alt { get; set; }
        public string Tags { get; set; }
        [Display(Name="Photo of the Month")]
        public bool IsPhotoOfTheMonth { get; set; }
        [Display(Name = "Photo of the Year")]
        public bool IsPhotoOfTheYear { get; set; }
        [Display(Name = "Photo")]
        public ImageViewModel PhotoImage { get; set; }
        [Display(Name = "Thumbnail")]
        public ImageViewModel ThumbnailImage { get; set; }
        public string AuthenticatedUserName { get; set; }
        [Display(Name = "Rating")]
        [UIHint("RateIt")]
        public double AuthenticatedUserRating { get; set; }
        [Display(Name = "Rating")]
        //[DisplayFormat(DataFormatString = "{0:F1}")]
        [DisplayFormat(DataFormatString = "{0:0.0}", NullDisplayText = "-")]
        public double AveRating { get; set; }
        public string UserName { get; set; }

        //// PostedFile is needed for Photos/Add and Photos/Edit. The uploaded photo
        //// is retreived from this property. We also use this property to specify
        //// the filesize limit in kB.
        //// Check to see if it is required or not in Validate().
        //[FileSize(512)]
        //[Display(Name = "Photo")]
        //[RequiredIf("CallingAction", "Create")]
        //public HttpPostedFileBase PostedFile { get; set; }
        //public string CallingAction { get; set; }

        #region constructors
        public PhotoViewModel() { }

        public PhotoViewModel(Photo photoModel)
        {
            this.ProjectId = photoModel.ProjectId;
            this.PhotoId = photoModel.PhotoId;
            this.Date = photoModel.Date;
            this.Title = photoModel.Title;
            this.Alt = photoModel.Alt;
            this.Tags = photoModel.Tags;
            this.IsPhotoOfTheMonth = photoModel.IsPhotoOfTheMonth;
            this.IsPhotoOfTheYear = photoModel.IsPhotoOfTheYear;
            this.UserName = photoModel.Project.IdentityUserName;
            this.PhotoImage = new ImageViewModel
            {
                ContentType = photoModel.PhotoImage.ContentType,
                Bytes = photoModel.PhotoImage.Bytes
            };
            this.ThumbnailImage = new ImageViewModel
            {
                ContentType = photoModel.ThumbnailImage.ContentType,
                Bytes = photoModel.ThumbnailImage.Bytes
            };
        }

        // Copy constructor
        public PhotoViewModel(PhotoViewModel photoModel)
        {
            this.ProjectId = photoModel.ProjectId;
            this.PhotoId = photoModel.PhotoId;
            this.AuthenticatedUserName = photoModel.AuthenticatedUserName;
            this.AuthenticatedUserRating = photoModel.AuthenticatedUserRating;
            this.AveRating = photoModel.AveRating;
            this.Date = photoModel.Date;
            this.Title = photoModel.Title;
            this.Alt = photoModel.Alt;
            this.Tags = photoModel.Tags;
            this.IsPhotoOfTheMonth = photoModel.IsPhotoOfTheMonth;
            this.IsPhotoOfTheYear = photoModel.IsPhotoOfTheYear;
            this.PhotoImage = photoModel.PhotoImage;
            this.ThumbnailImage = photoModel.ThumbnailImage;
            this.UserName = photoModel.UserName;
        }
        #endregion

        /// <summary>
        /// Gets the Url to this photo based on the route.
        /// </summary>
        /// <returns>Returns the Url to the photo in the form /username/2016/3/1 </returns>
        public string GetUrl()
        {
            // Important to pass the request context to the constructor. 
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var url = urlHelper.RouteUrl("DayView", new
            {
                username = UserName,
                year = Date.ToString("yyyy"),
                month = Date.ToString("%M"), // Use % to specify a custom formatted string (not the standard formatting M)
                day = Date.ToString("%d") // Use % to specify a custom formatted string (not the standard formatting d)
            });

            return url;
        }

        /// <summary>
        /// Gets the Url to this photo based on the route.
        /// </summary>
        /// <returns>Returns the Url to the photo in the form /username/2016/3/1 </returns>
        public string GetUrl(string username)
        {
            // Important to pass the request context to the constructor. 
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var url = urlHelper.RouteUrl("DayView", new
            {
                username = username,
                year = Date.ToString("yyyy"),
                month = Date.ToString("%M"), // Use % to specify a custom formatted string (not the standard formatting M)
                day = Date.ToString("%d") // Use % to specify a custom formatted string (not the standard formatting d)
            });

            return url;
        }
    }
}