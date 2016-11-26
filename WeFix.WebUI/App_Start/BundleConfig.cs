using System.Web;
using System.Web.Optimization;

namespace WeFix.WebUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
               bundles.Add(new StyleBundle("~/Bundles/css")
               .Include("~/Content/css/bootstrap.css")
               .Include("~/Content/css/bootstrap-social.css")
               .Include("~/Content/css/datepicker3.css")
               .Include("~/Content/css/AdminLTE.css")
               .Include("~/Content/css/skins/skin-black.css")
               .Include("~/Content/css/jquery-ui.css"));

            bundles.Add(new ScriptBundle("~/Bundles/js")
                .Include("~/Content/js/plugins/fastclick/fastclick.js")
                .Include("~/Content/js/bootstrap.js")
                .Include("~/Content/js/plugins/moment/moment.js")
                .Include("~/Content/js/jquery-ui.js")
                .Include("~/Content/js/plugins/datepicker/bootstrap-datepicker.js")
                .Include("~/Content/js/plugins/icheck/icheck.js")
                .Include("~/Content/js/app.js")
                .Include("~/Content/js/init.js"));

            bundles.Add(new StyleBundle("~/Bundles/Admincss")
               .Include("~/Content/Admin/css/AdminLTE.css")
               .Include("~/Content/Admin/css/skins/skin-blue.css"));

            bundles.Add(new ScriptBundle("~/Bundles/Adminjs")
                .Include("~/Content/js/jquery-ui.js")
                .Include("~/Content/Admin/js/plugins/bootstrap/bootstrap.js")
                .Include("~/Content/Admin/js/plugins/fastclick/fastclick.js")
                .Include("~/Content/Admin/js/plugins/slimscroll/jquery.slimscroll.js")
                .Include("~/Content/Admin/js/plugins/moment/moment.js")
                .Include("~/Content/Admin/js/plugins/datepicker/bootstrap-datepicker.js")
                .Include("~/Content/Admin/js/app.js")
                .Include("~/Content/Admin/js/init.js"));

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
