using System.Web;
using System.Web.Optimization;

namespace PaD
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.validate*",
                    "~/Scripts/pad-validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/pad-common").Include(
                    "~/Scripts/pad-common.js"));

            //bundles.Add(new ScriptBundle("~/bundles/mvcfoolproof").Include(
            //        "~/Scripts/MicrosoftAjax*",
            //        "~/Scripts/MicrosoftMvcAjax*",
            //        "~/Scripts/MicrosoftMvcValidation*"));

            bundles.Add(new ScriptBundle("~/bundles/rateit").Include(
                    "~/Scripts/jquery.rateit*"));

            bundles.Add(new ScriptBundle("~/bundles/touchSwipe").Include(
                    "~/Scripts/jquery.touchSwipe*"));

            bundles.Add(new ScriptBundle("~/bundles/google-analytics").Include(
                    "~/Scripts/google-analytics*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                    "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/bootstrap.js"
                    //"~/Scripts/respond.js"));
                    ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datetimepicker").Include(
                    "~/Scripts/moment-with-locales*",
                    "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/bootstrap-datetimepicker.css",
                    "~/Content/font-awesome*",
                    "~/Content/rateit.css",
                    "~/Content/site.css",
                    "~/Content/pad-calendar.css"));
        }
    }
}
