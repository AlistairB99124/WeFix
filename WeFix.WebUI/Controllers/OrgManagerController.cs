using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeFix.WebUI.Controllers
{
    public class OrgManagerController : Controller
    {
        // GET: OrgManager
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DeptManagers()
        {
            return View();
        }
    }
}