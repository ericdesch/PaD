using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaD.ViewModels
{
    public class PhotoPagedListViewModel
    {
        public ICollection<DayViewModel> DayViewModels { get; set; }
        public StaticPagedList<DayViewModel> PhotoViewModelsList { get; set; }

        public PhotoPagedListViewModel()
        {
            this.DayViewModels = new HashSet<DayViewModel>();
        }

    }
}