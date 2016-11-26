using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WeFix.WebUI.Models;
using WeFix.Domain.Entities;
using WeFix.Domain.Context;
using System;
using System.Net;
using System.IO;
using System.Drawing;
using System.Data.Entity;

namespace WeFix.WebUI.Controllers
{
    /// <summary>
    /// The department manager controller provide the business logic to add, edit, delete and view managers
    /// </summary>
    /// <remarks>
    /// In most cases the controller interacts with the Organisations Dashboard so that organisation managers have the ability to CRUD their Department Managers
    /// </remarks>
    public class DepartmentManagersController : Controller
    {
        private SignInManager _signInManager;
        private UserManager _userManager;
        private EFDbContext db = new EFDbContext();
        private const string ExpectedImagePrefixJpeg = "data:image/jpeg;base64,";
        private const string ExpectedImagePrefixPng = "data:image/png;base64,";

        public DepartmentManagersController()
        {
        }

        public DepartmentManagersController(UserManager userManager, SignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public SignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<SignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public UserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: DepartmentManagers
        public ActionResult Index()
        {
            var departmentManagers = db.DepartmentManagers.Include(d => d.Department).Include(d => d.User);
            return View(departmentManagers.ToList());
        }

        // GET: DepartmentManagers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            DepartmentManager departmentManager = db.DepartmentManagers.Find(id);
            if (departmentManager == null)
            {
                return HttpNotFound();
            }
            return View(departmentManager);
        }

        // GET: DepartmentManagers/Create
        public ActionResult Create()
        {
          
            return View();
        }

        // POST: DepartmentManagers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentManagerId,UserId,DepartmentId,Position")] WeFix.WebUI.Models.DashboardViewModel departmentManager, FormCollection collection)
        {
            try
            {
                int deptId = Convert.ToInt32(collection.Get("deptId"));
                string userId = Convert.ToString(collection.Get("ManagerUserId"));
                DepartmentManager deptMan = new DepartmentManager()
                {
                     DepartmentId =deptId,
                      Position = departmentManager.Position,
                       UserId = userId                                           
                };
                db.DepartmentManagers.Add(deptMan);
                db.SaveChanges();
                return RedirectToAction("Management","Organisations",null);
            }
            catch
            {
                return RedirectToAction("Management", "Organisations", null);
            }                  
            
        }

        // GET: DepartmentManagers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            DepartmentManager departmentManager = db.DepartmentManagers.Find(id);
            if (departmentManager == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", departmentManager.DepartmentId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", departmentManager.UserId);
            return View(departmentManager);
        }

        // POST: DepartmentManagers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentManagerId,UserId,DepartmentId,Position")] DepartmentManager departmentManager)
        {
            if (ModelState.IsValid)
            {
                db.Entry(departmentManager).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Management","Organisations",null);
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", departmentManager.DepartmentId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", departmentManager.UserId);
            return View(departmentManager);
        }

        // GET: DepartmentManagers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            DepartmentManager departmentManager = db.DepartmentManagers.Find(id);
            if (departmentManager == null)
            {
                return HttpNotFound();
            }
            return View(departmentManager);
        }

        // POST: DepartmentManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DepartmentManager departmentManager = db.DepartmentManagers.Find(id);
            db.DepartmentManagers.Remove(departmentManager);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteFromDash(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            DepartmentManager departmentManager = db.DepartmentManagers.Find(id);
            if (departmentManager == null)
            {
                return HttpNotFound();
            }
            User user = db.Users.Find(departmentManager.UserId);
            db.DepartmentManagers.Remove(departmentManager);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Management", "Organisations", null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateWithUser(DashboardViewModel model, FormCollection collection)
        {
            try
            {
                string code = collection.Get("CountryCode");
                string cell = string.Empty;
                if (model.Cell.StartsWith("0"))
                {
                    cell = code + model.Cell.Substring(0, 1);
                }else
                {
                    cell = code + model.Cell;
                }
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Cell = cell,
                    PhoneNumber = cell,
                    IsManager = true,
                    ConnectionId ="",
                    EmailConfirmed =true,
                    IsOnline =false,
                    LastOnline =DateTime.Now,
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string data_uri = collection.Get("base64");
                    string userId = string.Empty;
                    while (userId == string.Empty)
                    {
                        userId = (from x in db.Users where x.Id == user.Id select x.Id).FirstOrDefault();
                    }

                    User newUser = (from x in db.Users where x.Id == userId select x).FirstOrDefault();
                    DepartmentManager deptManager = new DepartmentManager()
                    {
                        DepartmentId = model.DepartmentId,
                        Position = model.Position,
                        UserId = newUser.Id
                    };
                    db.DepartmentManagers.Add(deptManager);
                    
                    if (data_uri != "")
                    {
                        string Base64 = string.Empty;
                        if (data_uri.Contains(ExpectedImagePrefixJpeg))
                        {
                            Base64 = data_uri.Substring(ExpectedImagePrefixJpeg.Length);
                        }
                        else
                        {
                            Base64 = data_uri.Substring(ExpectedImagePrefixPng.Length);
                        }
                        
                        byte[] bytes = Convert.FromBase64String(Base64);
                        string directory = "~/Content/uploads/profiles/" + user.Id + "/";
                        Image image;
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            image = Image.FromStream(ms);
                            if (Directory.Exists(directory))
                            {
                                directory.Substring(directory.Length - 1);
                                image.Save(Server.MapPath(directory + "Profile.jpg"));
                                newUser.UserPhotoUrl = directory + "Profile.jpg";
                            }
                            else
                            {
                                Directory.CreateDirectory(Server.MapPath(directory));
                                directory.Substring(directory.Length - 1);
                                image.Save(Server.MapPath(directory + "Profile.jpg"));
                                newUser.UserPhotoUrl = directory + "Profile.jpg";
                            }
                        }
                    }
                    db.Entry(newUser).State = EntityState.Modified;
                    db.SaveChanges();
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Management", "Organisations", null);
                }
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return RedirectToAction("Management", "Organisations", null);
            }
            return View(model);
        }
    }
}
