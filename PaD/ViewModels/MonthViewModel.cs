using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaD.ViewModels
{
    public class MonthViewModel : ProjectViewModel
    {
        public int Month { get; set;  }
        public int Year { get; set; }

        public MonthViewModel()
            : base()
        {
        }
    }
}