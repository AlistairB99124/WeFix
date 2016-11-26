using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeFix.Domain.Context;
using WeFix.Domain.Entities;
using WeFix.Logic;

namespace WeFix.WebUI.Controllers
{
    public class OrganisationsController : Controller
    {
        private EFDbContext db = new EFDbContext();
        

        public ActionResult ErrorNotApproved(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Organisation organisation = db.Organisations.Find(id);
            if (organisation == null)
            {
                return HttpNotFound();
            }
            ViewBag.Message = "Organistion, " + organisation.Name + " has not been approved yet. We are sorry for the inconvenience. We shall respond shortly.";
            return View();
        }
        public ActionResult Dashboard()
        {
            var userString = HttpContext.User.Identity.GetUserId();
            OrganisationManager orgMan = (from x in db.OrganisationManagers where x.UserId == userString select x).FirstOrDefault();
            var depMan = (from x in db.DepartmentManagers where x.UserId == userString select x).FirstOrDefault();
            if (orgMan != null)
            {
                Organisation org = (from x in db.Organisations where x.OrganisationId == orgMan.OrganisationId select x).FirstOrDefault();
                if (org.Approved)
                {
                    var suburbs = (from b in db.ReportSuburbs select b).ToList();
                    var faults = (from q in db.Faults select q).ToList();
                    var chartValues = suburbs.GroupBy(o => o.Suburb).Select(o => new { xVal = o.Key, yVal = o.Distinct().Count() });
                    var getVals = (from u in faults orderby u.DateCreated ascending group u by u.DateCreated.Date into t select new { day = t.Key, count = t.Count() });
                  
                    var lineChartValues = getVals;
                    //Create a string that will act as the javascipt that will populate charts in the view
                    StringBuilder lineChartlabels = new StringBuilder();
                    StringBuilder lineChartQuantities = new StringBuilder();
                    StringBuilder pieData = new StringBuilder();
                    //the data needs to be started with a label id                    
                    lineChartlabels.Append("labels:[");
                    lineChartQuantities.Append("data: [");
                    pieData.Append("var PieData = [");
                    //Populate data from the x and y values that you have determined from your objects
                    foreach (var chartVal in chartValues)
                    {
                        var color = "#" + Guid.NewGuid().ToString().Substring(0, 6);
                        var colorHighlight = "#" + Guid.NewGuid().ToString().Substring(0, 6);
                        //add each chart value to each label value

                        pieData.Append("{value:" + chartVal.yVal + ",color:'" + color + "',highlight:'" + colorHighlight + "',label:'" + chartVal.xVal + "'},");
                    }
                    foreach (var chartLineVal in lineChartValues)
                    {
                        lineChartlabels.Append("'" + chartLineVal.day.ToShortDateString() + "',");
                        lineChartQuantities.Append("'" + chartLineVal.count + "',");
                    }
                    //Since we had to add a comma to the end of each x/y value. We have to remove the 
                    //last comma so it makes semantic sense
                    //More data to add so "]," needs to be added
                    string lineChartlabelsResult = lineChartlabels.ToString().Remove(lineChartlabels.ToString().Length - 1, 0) + "],";
                    //This is the last data to be added, so no comma needed
                    string lineChartQuantitiesResult = lineChartQuantities.ToString().Remove(lineChartQuantities.ToString().Length - 1, 0) + "]";
                    string pieResult = pieData.ToString().Remove(pieData.ToString().Length - 1, 0) + "];";

                    //Append to View
                    ViewBag.Labels = lineChartlabelsResult.ToString();
                    ViewBag.Quantities = lineChartQuantitiesResult.ToString();
                    ViewBag.PieData = pieResult.ToString();

                    //Create Map Canvas and populate markers
                    IEnumerable<Fault> inFaults = (from t in db.Faults select t).ToList();
                    IEnumerable<Category> categories = (from q in db.Categories select q).ToList();
                    double x = 0;
                    double y = 0;
                    double z = 0;

                    foreach (Fault f in inFaults)
                    {
                        var latitude = f.Latitude * Math.PI / 180; ;
                        var longitude = f.Longitude * Math.PI / 180;
                        x += Math.Cos(latitude) * Math.Cos(longitude);
                        y += Math.Cos(latitude) * Math.Sin(longitude);
                        z += Math.Sin(latitude);
                    }
                    var total = inFaults.Count();
                    x = x / total;
                    y = y / total;
                    z = z / total;

                    var centralLongitude = (Math.Atan2(y, x)) / (Math.PI / 180);
                    var centralSquareRoot = Math.Sqrt(x * x + y * y);
                    var centralLatitude = (Math.Atan2(z, centralSquareRoot)) / (Math.PI / 180);
                    #region Javascript String
                    //Generate javascipt to render google map
                    StringBuilder sb = new StringBuilder();
                    sb.Append("var map; function initMap(){");
                    sb.Append("map = new google.maps.Map(document.getElementById('map'), {");
                    sb.Append("zoom: 12,");
                    //Input central coordinates for initial state of the map
                    sb.Append("center: new google.maps.LatLng(" + centralLatitude.ToString().Replace(",", ".") + ", " + centralLongitude.ToString().Replace(",", ".") + "), ");
                    sb.Append("mapTypeId: google.maps.MapTypeId.TERRAIN");
                    sb.Append("});");
                    //Get Icon resource
                    sb.Append("var iconBase = '../Content/img/';");
                    sb.Append("var icons = {");
                    foreach (Category c in categories)
                    {
                        sb.Append(c.Name.ToString().Replace(" ", "") + ": {");
                        sb.Append("icon: iconBase + '" + c.Image.Replace(".png","Pin.png") + "'");
                        sb.Append("},");
                    }
                    var ind = sb.ToString().LastIndexOf(',');
                    if (ind >= 0) { sb.Remove(ind, 1); }
                    sb.Append(" };");
                    sb.Append("function addMarker(feature) {");
                    sb.Append("var marker = new google.maps.Marker({");
                    sb.Append("position: feature.position,");
                    sb.Append("icon: icons[feature.type].icon,");
                    sb.Append("map: map,");
                    sb.Append("opacity: feature.opacity");
                    sb.Append(" });");
                    sb.Append("}");
                    //Get marker coordinates for each fault
                    sb.Append("var features = [");
                    foreach (Fault f in inFaults)
                    {
                        Category cat = db.Categories.Find(f.CategoryId);
                        sb.Append("{");
                        sb.Append("position: new google.maps.LatLng(" + f.Latitude.ToString().Replace(",", ".") + ", " + f.Longitude.ToString().Replace(",", ".") + "),");
                        sb.Append("type: '" + cat.Name.ToString().Replace(" ", "") + "',");
                        var severity = f.SeverityId;
                        switch (severity)
                        {
                            case 1:
                                sb.Append("opacity: " + (0.1).ToString().Replace(",", "."));
                                break;
                            case 2:
                                sb.Append("opacity: " + (0.2).ToString().Replace(",", "."));
                                break;
                            case 3:
                                sb.Append("opacity: " + (0.3).ToString().Replace(",", "."));
                                break;
                            case 4:
                                sb.Append("opacity: " + (0.4).ToString().Replace(",", "."));
                                break;
                            case 5:
                                sb.Append("opacity: " + (0.5).ToString().Replace(",", "."));
                                break;
                            case 6:
                                sb.Append("opacity: " + (0.6).ToString().Replace(",", "."));
                                break;
                            case 7:
                                sb.Append("opacity: " + (0.7).ToString().Replace(",", "."));
                                break;
                            case 8:
                                sb.Append("opacity: " + (0.8).ToString().Replace(",", "."));
                                break;
                            case 9:
                                sb.Append("opacity: " + (0.9).ToString().Replace(",", "."));
                                break;
                            case 10:
                                sb.Append("opacity: " + 1);
                                break;
                            default:
                                sb.Append("opacity: " + 1);
                                break;
                        }
                        sb.Append("},");
                    }
                    var index = sb.ToString().LastIndexOf(',');
                    if (index >= 0) { sb.Remove(index, 1); }
                    sb.Append("];");
                    sb.Append("for (var i = 0, feature; feature = features[i]; i++) {");
                    sb.Append("addMarker(feature);");
                    sb.Append("}}");
                    #endregion

                    //Supply view variables
                    #region                    
                    var helper = new WeFix.Logic.Helpers();
                    string userId = HttpContext.User.Identity.GetUserId();
                    User user = db.Users.Find(userId);
                    Organisation organisation;
                    if (orgMan != null)
                    {
                        organisation = db.Organisations.Find(orgMan.OrganisationId);
                    }
                    else
                    {
                        var department = (from w in db.Departments where w.DepartmentId == depMan.DepartmentId select w).FirstOrDefault();
                        organisation = db.Organisations.Find(department.OrganisationId);
                    }

                    var departments = (from w in db.Departments where w.OrganisationId == organisation.OrganisationId select w).ToList();
                    var allFaults = (from w in db.Faults select w).ToList();
                    var resolvedFaults = helper.GetRelevantFaults(db, organisation, allFaults, true);
                    var unresolvedFaults = helper.GetRelevantFaults(db, organisation, allFaults, false);
                    double unassignmentRate;
                    double responseRate;
                    if ((resolvedFaults.Count() + unresolvedFaults.Count()) != 0)
                    {
                        unassignmentRate = ((from w in resolvedFaults where w.ManagerId != null select w).ToList().Count + (from w in resolvedFaults where w.ManagerId != null select w).ToList().Count) / (resolvedFaults.Count() + unresolvedFaults.Count());
                        double c = Convert.ToDouble((from w in resolvedFaults where w.ManagerId != null select w).ToList().Count);
                        double d = Convert.ToDouble((from w in unresolvedFaults where w.ManagerId != null select w).ToList().Count);
                        double a = Convert.ToDouble(resolvedFaults.Count());
                        double b = Convert.ToDouble(unresolvedFaults.Count());
                        responseRate = (a / (a + b))*100;
                        unassignmentRate = ((c + d) / (a + b))*100;
                    }
                    else
                    {
                        unassignmentRate = 0;
                        responseRate = 0;
                    }

                    int totalUsers = db.Users.Count();
                    int countNewFaults = (from w in db.Faults where DbFunctions.DiffDays(w.DateCreated, DateTime.Now) > 7 select w).ToList().Count;

                    #endregion

                    ViewBag.CountNewFaults = countNewFaults;
                    ViewBag.ResponseRate = responseRate.ToString("#0.##");
                    ViewBag.TotalUsers = totalUsers;
                    ViewBag.AllFaults = allFaults;
                    ViewBag.User = user;
                    ViewBag.UnassignmentRate = unassignmentRate.ToString("#0.##");
                    ViewBag.MapScript = sb.ToString();
                    return View();
                }
                else
                {
                    return RedirectToAction("ErrorNotApproved", new { @id = org.OrganisationId });
                }
            }
            else if(depMan!=null)
            {
                    var suburbs = (from b in db.ReportSuburbs select b).ToList();
                    var faults = (from q in db.Faults select q).ToList();
                    var chartValues = suburbs.GroupBy(o => o.Suburb).Select(o => new { xVal = o.Key, yVal = o.Distinct().Count() });
                    var getVals = (from u in faults orderby u.DateCreated ascending group u by u.DateCreated.Date into t select new { day = t.Key, count = t.Count() });

                    var lineChartValues = getVals;
                    //Create a string that will act as the javascipt that will populate charts in the view
                    StringBuilder lineChartlabels = new StringBuilder();
                    StringBuilder lineChartQuantities = new StringBuilder();
                    StringBuilder pieData = new StringBuilder();
                    //the data needs to be started with a label id                    
                    lineChartlabels.Append("labels:[");                    
                    lineChartQuantities.Append("data: [");
                    pieData.Append("var PieData = [");
                    //Populate data from the x and y values that you have determined from your objects
                    foreach (var chartVal in chartValues)
                    {
                        var color = "#" + Guid.NewGuid().ToString().Substring(0, 6);
                        var colorHighlight = "#" + Guid.NewGuid().ToString().Substring(0, 6);
                        //add each chart value to each label value
                       
                        pieData.Append("{value:" + chartVal.yVal + ",color:'"+color+"',highlight:'"+colorHighlight+"',label:'" + chartVal.xVal + "'},");
                    }
                    foreach(var chartLineVal in lineChartValues)
                    {
                        lineChartlabels.Append("'" + chartLineVal.day.ToShortDateString() + "',");
                        lineChartQuantities.Append("'" + chartLineVal.count + "',");
                    }
                    //Since we had to add a comma to the end of each x/y value. We have to remove the 
                    //last comma so it makes semantic sense
                    //More data to add so "]," needs to be added
                    string lineChartlabelsResult = lineChartlabels.ToString().Remove(lineChartlabels.ToString().Length - 1, 0) + "],";
                    //This is the last data to be added, so no comma needed
                    string lineChartQuantitiesResult = lineChartQuantities.ToString().Remove(lineChartQuantities.ToString().Length - 1,0) + "]";
                    string pieResult = pieData.ToString().Remove(pieData.ToString().Length - 1, 0) + "];";

                    //Append to View
                    ViewBag.Labels = lineChartlabelsResult.ToString();
                    ViewBag.Quantities = lineChartQuantitiesResult.ToString();
                    ViewBag.PieData = pieResult.ToString();

                    //Create Map Canvas and populate markers
                    IEnumerable<Fault> inFaults = (from t in db.Faults select t).ToList();
                    IEnumerable<Category> categories = (from q in db.Categories select q).ToList();
                    double x = 0;
                    double y = 0;
                    double z = 0;

                    foreach (Fault f in inFaults)
                    {
                        var latitude = f.Latitude * Math.PI / 180; ;
                        var longitude = f.Longitude * Math.PI / 180;
                        x += Math.Cos(latitude) * Math.Cos(longitude);
                        y += Math.Cos(latitude) * Math.Sin(longitude);
                        z += Math.Sin(latitude);
                    }
                    var total = inFaults.Count();
                    x = x / total;
                    y = y / total;
                    z = z / total;

                    var centralLongitude = (Math.Atan2(y, x)) / (Math.PI / 180);
                    var centralSquareRoot = Math.Sqrt(x * x + y * y);
                    var centralLatitude = (Math.Atan2(z, centralSquareRoot)) / (Math.PI / 180);
                    #region Javascript String
                    //Generate javascipt to render google map
                    StringBuilder sb = new StringBuilder();
                    sb.Append("var map; initMap(){");
                    sb.Append("map = new google.maps.Map(document.getElementById('map'), {");
                    sb.Append("zoom: 12,");
                    //Input central coordinates for initial state of the map
                    sb.Append("center: new google.maps.LatLng(" + centralLatitude.ToString().Replace(",", ".") + ", " + centralLongitude.ToString().Replace(",", ".") + "), ");
                    sb.Append("mapTypeId: google.maps.MapTypeId.TERRAIN");
                    sb.Append("});");
                    //Get Icon resource
                    sb.Append("var iconBase = '../Content/img/';");
                    sb.Append("var icons = {");
                    foreach (Category c in categories)
                    {
                        sb.Append(c.Name.ToString().Replace(" ", "") + ": {");
                        sb.Append("icon: iconBase + '" + c.Image.Replace(".png","Pin.png") + "'");
                        sb.Append("},");
                    }
                    var ind = sb.ToString().LastIndexOf(',');
                    if (ind >= 0) { sb.Remove(ind, 1); }
                    sb.Append(" };");
                    sb.Append("function addMarker(feature) {");
                    sb.Append("var marker = new google.maps.Marker({");
                    sb.Append("position: feature.position,");
                    sb.Append("icon: icons[feature.type].icon,");
                    sb.Append("map: map,");
                    sb.Append("opacity: feature.opacity");
                    sb.Append(" });");
                    sb.Append("}");
                    //Get marker coordinates for each fault
                    sb.Append("var features = [");
                    foreach (Fault f in inFaults)
                    {
                        Category cat = db.Categories.Find(f.CategoryId);
                        sb.Append("{");
                        sb.Append("position: new google.maps.LatLng(" + f.Latitude.ToString().Replace(",", ".") + ", " + f.Longitude.ToString().Replace(",", ".") + "),");
                        sb.Append("type: '" + cat.Name.ToString().Replace(" ", "") + "',");
                        var severity = f.SeverityId;
                        switch (severity)
                        {
                            case 1:
                                sb.Append("opacity: " + (0.1).ToString().Replace(",", "."));
                                break;
                            case 2:
                                sb.Append("opacity: " + (0.2).ToString().Replace(",", "."));
                                break;
                            case 3:
                                sb.Append("opacity: " + (0.3).ToString().Replace(",", "."));
                                break;
                            case 4:
                                sb.Append("opacity: " + (0.4).ToString().Replace(",", "."));
                                break;
                            case 5:
                                sb.Append("opacity: " + (0.5).ToString().Replace(",", "."));
                                break;
                            case 6:
                                sb.Append("opacity: " + (0.6).ToString().Replace(",", "."));
                                break;
                            case 7:
                                sb.Append("opacity: " + (0.7).ToString().Replace(",", "."));
                                break;
                            case 8:
                                sb.Append("opacity: " + (0.8).ToString().Replace(",", "."));
                                break;
                            case 9:
                                sb.Append("opacity: " + (0.9).ToString().Replace(",", "."));
                                break;
                            case 10:
                                sb.Append("opacity: " + 1);
                                break;
                            default:
                                sb.Append("opacity: " + 1);
                                break;
                        }
                        sb.Append("},");
                    }
                    var index = sb.ToString().LastIndexOf(',');
                    if (index >= 0) { sb.Remove(index, 1); }
                    sb.Append("];");
                    sb.Append("for (var i = 0, feature; feature = features[i]; i++) {");
                    sb.Append("addMarker(feature);");
                    sb.Append("}}");
                    #endregion

                    ViewBag.MapScript = sb.ToString();
                    return View();
                
            }
            else
            {
                return RedirectToAction("Login", "Account", null);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult ContactAdmin(FormCollection collection)
        {
            string subject = collection.Get("subject");
            string message = collection.Get("Message");
            string userId = User.Identity.GetUserId();
            User user = db.Users.Find(userId);
            string fromAddress = user.Email;
            Email email = new Email();
            email.ProcessContact(fromAddress, subject, message);
            return RedirectToAction("Dashboard");
        }

        [Authorize]
        [HttpGet]
        public ActionResult AssignManager(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Fault fault = db.Faults.Find(id);
            if (fault == null)
            {
                return HttpNotFound();
            }
            var userId = HttpContext.User.Identity.GetUserId();
            var orgMan = (from x in db.OrganisationManagers where x.UserId == userId select x).FirstOrDefault();
            if (orgMan == null)
            {
                ViewBag.Error = "You are not authorised to execute this action";
            }
            else
            {
                var org = (from x in db.Organisations where x.OrganisationId == orgMan.OrganisationId select x).FirstOrDefault();
                var orgsDepartments = (from x in db.Departments where x.OrganisationId == org.OrganisationId select x).ToList();
                foreach(var dept in orgsDepartments)
                {
                    if(dept.CategoryId == fault.CategoryId)
                    {
                        var manager = (from x in db.DepartmentManagers where x.DepartmentId == dept.DepartmentId select x).FirstOrDefault();
                        fault.ManagerId = manager.DepartmentManagerId;
                        db.Entry(fault).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (fault.ManagerId == 0)
                {
                    ViewBag.Error = "You have no department to manage this type of fault, yet.";
                }
                else
                {
                    return RedirectToAction("Management", "Organisations", null);
                }
            }
            return RedirectToAction("Management", "Organisations", null);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AssignManagerIfInDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Fault fault = db.Faults.Find(id);
            if (fault == null)
            {
                return HttpNotFound();
            }
            var userId = HttpContext.User.Identity.GetUserId();
            var orgMan = (from x in db.OrganisationManagers where x.UserId == userId select x).FirstOrDefault();
            if (orgMan == null)
            {
                ViewBag.Error = "You are not authorised to execute this action";
            }
            else
            {
                var org = (from x in db.Organisations where x.OrganisationId == orgMan.OrganisationId select x).FirstOrDefault();
                var orgsDepartments = (from x in db.Departments where x.OrganisationId == org.OrganisationId select x).ToList();
                foreach (var dept in orgsDepartments)
                {
                    if (dept.CategoryId == fault.CategoryId)
                    {
                        var manager = (from x in db.DepartmentManagers where x.DepartmentId == dept.DepartmentId select x).FirstOrDefault();
                        fault.ManagerId = manager.DepartmentManagerId;
                        db.Entry(fault).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (fault.ManagerId == 0)
                {
                    ViewBag.Error = "You have no department to manage this type of fault, yet.";
                }
                else
                {
                    return RedirectToAction("Details", "Faults", new { id = fault.FaultId });
                }
            }
            return RedirectToAction("Management", "Organisations", null);
        }


        [Authorize]
        [HttpGet]
        public ActionResult UnAssignManager(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Fault fault = db.Faults.Find(id);
            if (fault == null)
            {
                return HttpNotFound();
            }
            var userId = HttpContext.User.Identity.GetUserId();
            var orgMan = (from x in db.OrganisationManagers where x.UserId == userId select x).FirstOrDefault();
            if (orgMan == null)
            {
                ViewBag.Error = "You are not authorised to execute this action";
            }
            else
            {
                var org = (from x in db.Organisations where x.OrganisationId == orgMan.OrganisationId select x).FirstOrDefault();
                var orgsDepartments = (from x in db.Departments where x.OrganisationId == org.OrganisationId select x).ToList();
                foreach (var dept in orgsDepartments)
                {
                    if (dept.CategoryId == fault.CategoryId)
                    {
                        
                        fault.ManagerId = null;
                        db.Entry(fault).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (fault.ManagerId == 0)
                {
                    ViewBag.Error = "You have no department to manage this type of fault, yet.";
                }
                else
                {
                    return RedirectToAction("Management", "Organisations", null);
                }
            }
            return RedirectToAction("Management", "Organisations", null);
        }

        [Authorize]
        [HttpGet]
        public ActionResult UnAssignManagerIfInDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Fault fault = db.Faults.Find(id);
            if (fault == null)
            {
                return HttpNotFound();
            }
            var userId = HttpContext.User.Identity.GetUserId();
            var orgMan = (from x in db.OrganisationManagers where x.UserId == userId select x).FirstOrDefault();
            if (orgMan == null)
            {
                ViewBag.Error = "You are not authorised to execute this action";
            }
            else
            {
                var org = (from x in db.Organisations where x.OrganisationId == orgMan.OrganisationId select x).FirstOrDefault();
                var orgsDepartments = (from x in db.Departments where x.OrganisationId == org.OrganisationId select x).ToList();
                foreach (var dept in orgsDepartments)
                {
                    if (dept.CategoryId == fault.CategoryId)
                    {

                        fault.ManagerId = null;
                        db.Entry(fault).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (fault.ManagerId == 0)
                {
                    ViewBag.Error = "You have no department to manage this type of fault, yet.";
                }
                else
                {
                    return RedirectToAction("Details", "Faults", new { id=fault.FaultId});
                }
            }
            return RedirectToAction("Management", "Organisations", null);
        }



        public ActionResult ConfirmOrganisation(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Organisation organisation = db.Organisations.Find(id);
            if (organisation == null)
            {
                return HttpNotFound();
            }
            try
            {
                organisation.Approved = true;
                db.Entry(organisation).State = EntityState.Modified;
                db.SaveChanges();
                Email email = new Email();
                email.ProcessOrgConfirmation(organisation, db);
                return RedirectToAction("Index", "Home", null);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult Management()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            OrganisationManager orgMan = (from x in db.OrganisationManagers where x.UserId == userId select x).FirstOrDefault();
            if (orgMan == null)
            {
                return RedirectToAction("Login", "Account", null);
            }
            else
            {
                var db = new EFDbContext();
                IEnumerable<Department> AllDept = null;
                IEnumerable<DepartmentManager> AllManagers = null;
                List<Fault> allFaults = null;
                IEnumerable<Fault> OrgFaultsUR = null;
                IEnumerable<Fault> OrgFaultsR = null;
                IEnumerable<DepartmentManager> OrgsManagers = null;
                List<DepartmentManager> orgsManagers = new List<DepartmentManager>();
                int orgId = 0;
                Organisation org = null;
                List<Fault> orgsFaults = new List<Fault>();
                IEnumerable<Country> countries = (from x in db.Countries select x).ToList();
                var selectCountry = new SelectList(countries, "CountryName", "CountryName", "South Africa");
                var selectCountryCode = new SelectList(countries, "CountryAccessCode", "CountryName", "+27");
                IEnumerable<Category> catsForSelect = (from x in db.Categories orderby x.Count descending select x).ToList();
                Category firstCat = (from x in catsForSelect select x).First();
                var CategorySelect = new SelectList(catsForSelect, "CategoryId", "Name", firstCat.CategoryId);
                if (userId != null)
                {
                    orgMan = (from x in db.OrganisationManagers where x.UserId == userId select x).FirstOrDefault();
                    if (orgMan != null)
                    {
                        org = (from x in db.Organisations where x.OrganisationId == orgMan.OrganisationId select x).FirstOrDefault();
                        orgId = org.OrganisationId;
                        AllManagers = (from x in db.DepartmentManagers select x).ToList();
                        AllDept = (from x in db.Departments where x.OrganisationId == orgId select x).ToList();
                        foreach (var dept in AllDept)
                        {
                            foreach (var man in AllManagers)
                            {
                                if (dept.DepartmentId == man.DepartmentId)
                                {
                                    orgsManagers.Add(man);
                                }
                            }
                        }
                        if (orgsManagers == null)
                        {
                            OrgsManagers = null;
                        }
                        else
                        {
                            OrgsManagers = orgsManagers as IEnumerable<DepartmentManager>;
                        }
                        allFaults = (from x in db.Faults select x).ToList();
                        var helper = new Helpers();
                        OrgFaultsUR = helper.GetRelevantFaults(db, org, allFaults, false);
                        OrgFaultsR = helper.GetRelevantFaults(db, org, allFaults, true);
                    }

                }
                Department selectDept = null;
                SelectList selectDepartment = null;
                if (AllDept.Any())
                {
                    selectDept = (from x in AllDept select x).First();
                    selectDepartment = new SelectList(AllDept, "DepartmentId", "Name", selectDept.DepartmentId);
                }
                if (org.Approved)
                {
                    ViewBag.SelectDept = selectDept;
                    ViewBag.SelectDepartment = selectDepartment;
                    ViewBag.OrgFaultsUR = OrgFaultsUR;
                    ViewBag.OrgFaultsR = OrgFaultsR;
                    ViewBag.AllDept = AllDept;
                    ViewBag.OrgsManagers = OrgsManagers;
                    ViewBag.CountrySelect = selectCountry;
                    ViewBag.CountryCodeSelect = selectCountryCode;
                    ViewBag.CategorySelect = CategorySelect;
                    ViewBag.Org = org;
                    return View();
                }
                else
                {                    
                    return RedirectToAction("ErrorNotApproved", new { @id = org.OrganisationId });
                }
            }
           
              
        }

        // GET: Organisations
        public ActionResult Index()
        {
            return View(db.Organisations.ToList());
        }

        // GET: Organisations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Organisation organisation = db.Organisations.Find(id);
            if (organisation == null)
            {
                return HttpNotFound();
            }
            return View(organisation);
        }

        // GET: Organisations/Create
        public ActionResult Create(int? id)
        {
            return View();
        }

        // POST: Organisations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrganisationId,Name,Approved")] Organisation organisation)
        {
            if (ModelState.IsValid)
            {
                db.Organisations.Add(organisation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(organisation);
        }

        // GET: Organisations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Organisation organisation = db.Organisations.Find(id);
            if (organisation == null)
            {
                return HttpNotFound();
            }
            return View(organisation);
        }

        // POST: Organisations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrganisationId,Name,Approved")] Organisation organisation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(organisation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organisation);
        }

        // GET: Organisations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Organisation organisation = db.Organisations.Find(id);
            if (organisation == null)
            {
                return HttpNotFound();
            }
            return View(organisation);
        }

        // POST: Organisations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Organisation organisation = db.Organisations.Find(id);
            db.Organisations.Remove(organisation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
