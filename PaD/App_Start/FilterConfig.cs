using System.Web;
using System.Web.Mvc;

using PaD.CustomFilters;
using PaD.DataContexts;
using PaD.Infrastructure;

namespace PaD
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new HandleConcurrencyExceptionFilter());
            //filters.Add(new CustomRequireHttpsFilter());
            //filters.Add(new TrackPageViewAttribute());
        }
    }
}
