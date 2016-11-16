using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaD.ViewModels
{
    public class DayViewModel
    {
        public string Username { get; set; }
        public DateTime ProjectFirstDate { get; set; }
        public DateTime ProjectLastDate { get; set; }
        public DateTime ProjectCurrentDate { get; set; }
        public PhotoViewModel PhotoViewModel { get; set; }
        [UIHint("RateIt")]
        public double AuthenticatedUserRating { get; set; }
    }
}