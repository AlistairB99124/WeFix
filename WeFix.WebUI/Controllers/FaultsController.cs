using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WeFix.Domain.Context;
using WeFix.Domain.Entities;
using WeFix.Logic;

namespace WeFix.WebUI.Controllers
{
    /// <summary>
    /// This controller performs the CRUD functionality for reporting faults
    /// </summary>
    /// <remarks>
    /// Due to the use of javascript, the use of FormCollection get values from hidden inputs in the View
    /// </remarks>
    public class FaultsController : Controller
    {
        private EFDbContext db = new EFDbContext();
        /// <summary>
        /// Constant used to remove the unnecessary string in the base64 string grabbed from the file upload
        /// </summary>
        private const string ExpectedImagePrefixJpeg = "data:image/jpeg;base64,";
        private const string ExpectedImagePrefixPng = "data:image/png;base64,";

        /// <summary>
        /// Get View if user exists
        /// </summary>
        /// <returns>
        /// View 'ReportFault'
        /// </returns>
        public ActionResult ReportFault()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var user = (from x in db.PublicUsers where x.UserId == userId select x.UserId).FirstOrDefault();
            ViewBag.UserId = user;
            if (user != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Manage", null);
            }
        }
        /// <summary>
        /// The main function of the app, to create a fault that will be emailed to the organisation managers
        /// </summary>
        /// <param name="fault">Fault Object Populated from View</param>
        /// <param name="collection">FormCollection to grab values from hidden inputs</param>
        /// <returns>
        /// Fault
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportFault([Bind(Include = "Description")] Fault fault, FormCollection collection)
        {
            //Get Latitude from hidden input of name='latitude'
            string lat = collection.Get("latitude");
            //Get Longitude value from hidden input of name='longitude'
            string lng = collection.Get("longitude");
            //Get the id of the logged on user
            string guid = HttpContext.User.Identity.GetUserId();
            //Since only public users may report faults, get an object of type PublicUser
            PublicUser user = (from x in db.PublicUsers where x.UserId == guid select x).FirstOrDefault();

            if (user != null)
            {
                /*-----------Add LatLng to Model-----------------*/
                //Due to some computer systems using commas instead of periods for decimal points
                //Therefore, check to see if "," exists. If so, replace with a period
                if (lat.Contains(","))
                {
                    lat.ToString().Replace(",", ".");
                    lng.ToString().Replace(",", ".");
                    //add values to the fault object. Data type must be double for best results for geolocation
                    //On a side note, no idea why I had to include CultureInfo, it works this way so fuck it.
                    fault.Latitude = Convert.ToDouble(lat, CultureInfo.InvariantCulture);
                    fault.Longitude = Convert.ToDouble(lng, CultureInfo.InvariantCulture);
                }
                else
                {
                    //Since we are dealing with periods, just add the values as is
                    fault.Latitude = Convert.ToDouble(lat, CultureInfo.InvariantCulture);
                    fault.Longitude = Convert.ToDouble(lng, CultureInfo.InvariantCulture);
                }

                /*---------Add Public User Id and Date Created-----------*/
                fault.PublicUserId = user.PublicUserId;
                fault.DateCreated = DateTime.Now;

                /*----------Add Category Id and Manager Id---------------*/
                //get category ID from formcollection with name="CategoryId"
                int categoryId = Convert.ToInt32(collection.Get("CategoryId"));
                fault.CategoryId = categoryId;

                /*------------Add Severity Id----------------------------*/
                //get severity level from formcollection with name="SeveritySlider"
                int SeverityLevel = Convert.ToInt32(collection.Get("SeveritySlider"));
                fault.SeverityId = SeverityLevel;

                /*----------------Add Upload image here------------------*/
                //So this was hard. Using HttpPostedFileBase should have worked. But no.
                //So I learnt that base64 string can can be grabbed from javascript and appended to
                //an hidden input where I grabbed in from the formcollection
                string data_uri = collection.Get("base64Label");
                //check that base64 is not null, else do not add to fault. It is nullable
                if (data_uri != "")
                {
                    //Remove the prefix from the base 64 string
                    string base64 = string.Empty;
                    if (data_uri.Contains(ExpectedImagePrefixJpeg))
                    {
                        base64 = data_uri.Substring(ExpectedImagePrefixJpeg.Length);
                    }
                    else
                    {
                        base64 = data_uri.Substring(ExpectedImagePrefixPng.Length);
                    }
                    //convert to byte array
                    byte[] bytes = Convert.FromBase64String(base64);
                    //declare the directory where the resulting image will be saved
                    string directory = "~/Content/img/faults/" + user.PublicUserId + "/";
                    
                    //Create the image object that will be the image
                    //Create the image object that will be the image
                    Image image;
                    Image thumbnail;
                    //Use MemoryStream to read byte array
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        //create the image
                        image = Image.FromStream(ms);
                        thumbnail = Image.FromStream(ms);
                        var imageRectangle = new Rectangle(0, 0, 85, 85);
                        var bitmapVersion = new Bitmap(image);
                        bitmapVersion = bitmapVersion.Clone(imageRectangle,PixelFormat.DontCare);
                        //Make sure that the directory where the image is to be store actually exists;
                        if (Directory.Exists(directory))
                        {
                            //We need a unique directory for the image. So I've used the public users ID and The date created
                            string date = (fault.DateCreated.Day.ToString() + fault.DateCreated.Month.ToString() + fault.DateCreated.Year.ToString() + fault.DateCreated.TimeOfDay.ToString().Replace(":", "")).Replace(".", "");
                            //Save the image to directory
                            image.Save(Server.MapPath(directory + date + ".jpg"),ImageFormat.Jpeg);
                            bitmapVersion.Save(Server.MapPath(directory + date + ".png"), ImageFormat.Png);
                            //Its called Mime type, buts just the URL where the image is located
                            fault.ImageURL = directory.Replace("~", "") + date + ".jpg";
                            fault.ImageThumbnail = directory.Replace("~", "") + date + ".png";
                        }
                        else
                        {
                            string date = fault.DateCreated.Day.ToString() + fault.DateCreated.Month.ToString() + fault.DateCreated.Year.ToString() + fault.DateCreated.TimeOfDay.ToString().Replace(":", "");
                            //Directory does not exist so damn well create it
                            Directory.CreateDirectory(Server.MapPath(directory));
                            image.Save(Server.MapPath(directory + date + ".jpg"),ImageFormat.Jpeg);
                            bitmapVersion.Save(Server.MapPath(directory + date + "..png"),ImageFormat.Png);
                            fault.ImageURL = directory.Replace("~", "") + date + ".jpg";
                            fault.ImageThumbnail = directory.Replace("~", "") + date + ".png";
                        }
                    }
                }
                /*-------------Add Sub-category------------------------*/
                //Get sub-category from formcollection with name='subCatId'
                int subCatId = Convert.ToInt32(collection.Get("subCatId"));
                //Sub-category is nullable so only add if it exists
                if (subCatId != 0)
                {
                    fault.SubCategoryId = subCatId;
                }
                /*-------------Add Progress Bools--------------*/
                fault.NotStarted = true;
                fault.InProgress = false;
                fault.Resolved = false;
                /*-------------Increment Count of Category Used--------------------------*/
                Category category = db.Categories.Find(fault.CategoryId);
                int count = category.Count + 1;
                category.Count = count;

                /*------------Save changes to Database-----------------*/
                try
                {
                    db.Faults.Add(fault);
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();

                    /*-----------Email Manager Fault Details-----------*/
                    Email EmailFault = new Email();
                    EmailFault.ProcesFault(fault, db);

                    //Finally Add Suburb to ReportSuburb Table
                    var helpers = new Helpers();
                    string suburb = helpers.GetSuburb(Convert.ToDouble(lat, CultureInfo.InvariantCulture), Convert.ToDouble(lng, CultureInfo.InvariantCulture));
                    ReportSuburb rSuburb = (from x in db.ReportSuburbs where x.Suburb == suburb select x).FirstOrDefault();
                    if (rSuburb == null)
                    {
                        ReportSuburb newSubReport = new ReportSuburb()
                        {
                            Suburb = suburb
                        };
                        db.ReportSuburbs.Add(newSubReport);
                        db.SaveChanges();
                    }
                    else
                    {
                        db.ReportSuburbs.Add(rSuburb);
                        db.SaveChanges();
                    }

                    //Redirect to Index
                    return RedirectToAction("ThanksForReporting",new { id = fault.FaultId });
                }
                catch(Exception ex)
                {
                    return RedirectToAction("TroubleShooting", "Home", ex);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account", null);
            }
        }


        [HttpGet]
        public ActionResult ThanksForReporting(int? Id)
        {
            if (Id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Fault fault = db.Faults.Find(Id);
            if (fault == null)
            {
                return HttpNotFound();
            }
            return View(fault);
        }

        [HttpPost]
        public ActionResult MakeComment(FormCollection collection)
        {
            try
            {
                int faultId = Convert.ToInt32(collection.Get("faultId"));
                string commentText = collection.Get("CommentText");

                FaultComment fComment = new FaultComment()
                {
                    FaultId = faultId,
                    Text = commentText,
                    TimeStamp = DateTime.Now
                };
                db.Comments.Add(fComment);
                db.SaveChanges();
                Fault fault = db.Faults.Find(faultId);
                if(fault.ManagerId != null)
                {
                    //Email Manager
                }
                return RedirectToAction("Index", "Manage", null);
            }
            catch
            {
                return RedirectToAction("Index", "Manage", null);
            }

        }

        // GET: Faults
        public ActionResult Index()
        {
            var faults = db.Faults.Include(f => f.Category).Include(f => f.PublicUser).Include(f => f.Severity);
            return View(faults.ToList());
        }

        public ActionResult OrgIndex()
        {
            var faults = db.Faults.Include(f => f.Category).Include(f => f.PublicUser).Include(f => f.Severity);
            return View(faults.ToList());
        }
        public ActionResult DeptIndex()
        {
            var faults = db.Faults.Include(f => f.Category).Include(f => f.PublicUser).Include(f => f.Severity);
            return View(faults.ToList());
        }
        public ActionResult PublicIndex()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var publicUser = (from x in db.PublicUsers where x.UserId == userId select x).FirstOrDefault();
            var faults = db.Faults.Include(f => f.Category).Include(f => f.PublicUser).Include(f => f.Severity);
            var usersFaults = (from x in faults where x.PublicUserId == publicUser.PublicUserId select x).ToList();
            return View(usersFaults);
        }
        [Authorize]
        [HttpGet]
        public ActionResult MarkResolved(int? id)
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
            try
            {
                if (fault.ManagerId != null)
                {
                    fault.Resolved = true;
                    db.Entry(fault).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Management", "Organisations", null);
                }
                else
                {
                    return RedirectToAction("Management", "Organisations", null);
                }              
            }
            catch
            {
                return RedirectToAction("Management", "Organisations", null);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult MarkResolvedIfInDetails(int? id)
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
            try
            {
                if (fault.ManagerId != null)
                {
                    fault.Resolved = true;
                    db.Entry(fault).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Faults", new { id = fault.FaultId });
                }
                else
                {
                    return RedirectToAction("Management", "Organisations", null);
                }
            }
            catch
            {
                return RedirectToAction("Management", "Organisations", null);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult MarkUnresolved(int? id)
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
            try
            {
                if (fault.ManagerId != null)
                {
                    fault.Resolved = false;
                    db.Entry(fault).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Management", "Organisations", null);
                }
                else
                {
                    return RedirectToAction("Management", "Organisations", null);
                }
               
            }
            catch
            {
                return RedirectToAction("Management", "Organisations", null);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult MarkUnresolvedIfInDetails(int? id)
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
            try
            {
                if (fault.ManagerId != null)
                {
                    fault.Resolved = false;
                    db.Entry(fault).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Faults", null);
                }
                else
                {
                    return RedirectToAction("Management", "Organisations", null);
                }

            }
            catch
            {
                return RedirectToAction("Management", "Organisations", null);
            }
        }



        // GET: Faults/Details/5
        public ActionResult Details(int? id)
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
            return View(fault);
        }

        public ActionResult DetailsForPublic(int? id)
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
            return View(fault);
        }

        // GET: Faults/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            ViewBag.PublicUserId = new SelectList(db.PublicUsers, "PublicUserId", "UserId");
            ViewBag.SeverityId = new SelectList(db.Severities, "SeverityId", "Level");
            return View();
        }

        // POST: Faults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FaultId,PublicUserId,DateCreated,Description,Latitude,Longitude,ImageData,ImageMimeType,ImageThumbnail,CategoryId,SeverityId,ManagerId,Resolved")] Fault fault)
        {
            if (ModelState.IsValid)
            {
                db.Faults.Add(fault);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", fault.CategoryId);
            ViewBag.PublicUserId = new SelectList(db.PublicUsers, "PublicUserId", "UserId", fault.PublicUserId);
            ViewBag.SeverityId = new SelectList(db.Severities, "SeverityId", "Level", fault.SeverityId);
            return View(fault);
        }

        // GET: Faults/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", fault.CategoryId);
            ViewBag.PublicUserId = new SelectList(db.PublicUsers, "PublicUserId", "UserId", fault.PublicUserId);
            ViewBag.SeverityId = new SelectList(db.Severities, "SeverityId", "Level", fault.SeverityId);
            return View(fault);
        }

        // POST: Faults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FaultId,PublicUserId,DateCreated,Description,Latitude,Longitude,ImageURL,ImageThumbnail,CategoryId,SubCategoryId,SeverityId,ManagerId,Resolved")] Fault fault)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fault).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("PublicIndex");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", fault.CategoryId);
            ViewBag.PublicUserId = new SelectList(db.PublicUsers, "PublicUserId", "UserId", fault.PublicUserId);
            ViewBag.SeverityId = new SelectList(db.Severities, "SeverityId", "Level", fault.SeverityId);
            return View(fault);
        }

        // GET: Faults/Delete/5
        public ActionResult Delete(int? id)
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
            return View(fault);
        }

        // POST: Faults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fault fault = db.Faults.Find(id);
            db.Faults.Remove(fault);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteFromDetails(int? id)
        {
            Fault fault = db.Faults.Find(id);
            db.Faults.Remove(fault);
            db.SaveChanges();
            return RedirectToAction("PublicIndex");
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
