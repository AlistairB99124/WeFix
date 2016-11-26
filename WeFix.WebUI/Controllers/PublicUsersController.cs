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

namespace WeFix.WebUI.Controllers
{
    public class PublicUsersController : Controller
    {
        private EFDbContext db = new EFDbContext();

        // GET: PublicUsers
        public ActionResult Index()
        {
            var publicUsers = db.PublicUsers.Include(p => p.User);
            return View(publicUsers.ToList());
        }

        // GET: PublicUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            PublicUser publicUser = db.PublicUsers.Find(id);
            if (publicUser == null)
            {
                return HttpNotFound();
            }
            return View(publicUser);
        }

        // GET: PublicUsers/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: PublicUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PublicUserId,UserId")] PublicUser publicUser)
        {
            if (ModelState.IsValid)
            {
                db.PublicUsers.Add(publicUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", publicUser.UserId);
            return View(publicUser);
        }

        // GET: PublicUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            PublicUser publicUser = db.PublicUsers.Find(id);
            if (publicUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", publicUser.UserId);
            return View(publicUser);
        }

        // POST: PublicUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PublicUserId,UserId")] PublicUser publicUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(publicUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", publicUser.UserId);
            return View(publicUser);
        }

        // GET: PublicUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            PublicUser publicUser = db.PublicUsers.Find(id);
            if (publicUser == null)
            {
                return HttpNotFound();
            }
            return View(publicUser);
        }

        // POST: PublicUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PublicUser publicUser = db.PublicUsers.Find(id);
            db.PublicUsers.Remove(publicUser);
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
