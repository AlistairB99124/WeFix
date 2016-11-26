using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WeFix.Domain.Context;
using WeFix.Domain.Entities;
using GoogleMaps.LocationServices;
using Microsoft.AspNet.Identity;
using System.Globalization;

namespace WeFix.WebUI.Controllers
{
    public class DepartmentsController : Controller
    {
        private EFDbContext db = new EFDbContext();

        // GET: Departments
        public ActionResult Index()
        {
            var departments = db.Departments.Include(d => d.Category).Include(d => d.Organisation);
            return View(departments.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create(int? id)
        {
            //ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentId,Name,AddressLine1,AddressLine2,City,Country,PostalCode,CategoryId,OrganisationId")] WeFix.WebUI.Models.DashboardViewModel department, FormCollection collection)
        {
            int orgId = Convert.ToInt32(collection.Get("OrgId"));
            AddressData address = new AddressData()
            {
                Address = department.AddressLine1 + ", " + department.AddressLine2,
                City = department.City,
                Country = department.Country,
                Zip = department.PostalCode,
                State = ""
            };
            GoogleLocationService gls = new GoogleLocationService();
            var latlng = gls.GetLatLongFromAddress(address);
            //var juris = Convert.ToDecimal("jurisdiction");
            Department dept = new Department()
            {
                AddressLine1 = department.AddressLine1,
                AddressLine2 = department.AddressLine2,
                CategoryId = department.CategoryId,
                City = department.City,
                Country = department.Country,
                Latitude = latlng.Latitude,
                Longitude = latlng.Longitude,
                Name = department.Name,
                OrganisationId = orgId,
                PostalCode = department.PostalCode
            };
            try
            {
                db.Departments.Add(dept);
                db.SaveChanges();
                return RedirectToAction("Management","Organisations",null);
            }
            catch
            {
                return RedirectToAction("Index", "Home", null);
            }
        }

        public ActionResult Management()
        {
            var userID = HttpContext.User.Identity.GetUserId();
            var manager = (from x in db.DepartmentManagers where x.UserId == userID select x).FirstOrDefault();
            if (manager == null)
            {
                ViewBag.Error = "You are not a department manager";
                return RedirectToAction("Index", "Home", null);
            }
            else
            {
                return View();
            }            
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", department.CategoryId);
            ViewBag.OrganisationId = new SelectList(db.Organisations, "OrganisationId", "Name", department.OrganisationId);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,Name,AddressLine1,AddressLine2,City,Country,PostalCode,CategoryId,OrganisationId")] Department department, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                int orgId = 0;
                orgId = Convert.ToInt32(collection.Get("OrgId"));
                department.OrganisationId = orgId;
                var gls = new GoogleLocationService();
                AddressData address = new AddressData()
                {
                    Address = department.AddressLine1 + "' " + department.AddressLine2,
                    City = department.City,
                    Country = department.Country,
                    State = "",
                    Zip = department.PostalCode
                };
                var latlng = gls.GetLatLongFromAddress(address);
               
                    department.Latitude = Convert.ToDouble(latlng.Latitude, CultureInfo.InvariantCulture);
                    department.Longitude = Convert.ToDouble(latlng.Longitude, CultureInfo.InvariantCulture);
                

                var jurisdiction = Convert.ToDecimal(collection.Get("jurisdiction"));

                department.Jurisdiction = jurisdiction;

                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Management","Organisations",null);
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", department.CategoryId);
            ViewBag.OrganisationId = new SelectList(db.Organisations, "OrganisationId", "Name", department.OrganisationId);
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteFromDash(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
            db.SaveChanges();
            return RedirectToAction("Management","Organisations",null);
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
