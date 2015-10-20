using System.Web;
using System.Web.Optimization;

namespace MobilePong.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // core libraries that must load first
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-1.9.1.min.js"));
            

            // external libraries
             //bundles.Add(new ScriptBundle("~/bundles/applib").Include(
             //   "~/
             //   ));

            // application scripts
            //bundles.Add(new ScriptBundle("~/bundles/applib").IncludeDirectory("~/Scripts/app/", "*.js", false));



            //bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
            /*bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));*/
        }
    }
}