using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WeFix.Domain.Context;
using WeFix.Domain.Entities;

namespace WeFix.WebUI.Controllers
{
    [Authorize]
    public class BlogsController : Controller
    {
        private EFDbContext db = new EFDbContext();
        private const string ExpectedImagePrefixJpeg = "data:image/jpeg;base64,";
        private const string ExpectedImagePrefixPng = "data:image/png;base64,";

        // GET: Blogs
        public ActionResult Index()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var blogs = (from x in db.Blogs where x.AuthorId == userId select x).ToList();
            return View(blogs);
        }

        // GET: Blogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Blogs/Create
        [ValidateInput(false)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Title,Article,Featured,CommentsEnabled,Enabaled")] Blog blog, FormCollection collection)
        {
            try
            {
                blog.Date_Published = DateTime.Now;                
                blog.AuthorId = HttpContext.User.Identity.GetUserId();
                
                var data_uri = collection.Get("base64");
                if (data_uri != string.Empty)
                {
                    string base64 = string.Empty;
                    if (data_uri.Contains(ExpectedImagePrefixJpeg))
                    {
                        base64 = data_uri.Substring(ExpectedImagePrefixJpeg.Length);
                    }
                    else
                    {
                        base64 = data_uri.Substring(ExpectedImagePrefixPng.Length);
                    }
                    byte[] bytes = Convert.FromBase64String(base64);
                    string directory = "~/Content/uploads/blogs/" + blog.AuthorId + "/";
                    Image image;
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        image = Image.FromStream(ms);
                        if (Directory.Exists(directory))
                        {
                            directory.Substring(directory.Length - 1);
                            image.Save(Server.MapPath(directory + "Banner-" + blog.Date_Published.ToString().Replace("/", "").Replace(":", "") + ".jpg"));
                            blog.BannerImage = directory + "Banner-" + blog.Date_Published.ToString().Replace("/", "").Replace(":", "") + ".jpg";
                        }
                        else
                        {
                            Directory.CreateDirectory(Server.MapPath(directory));
                            directory.Substring(directory.Length - 1);
                            image.Save(Server.MapPath(directory + "Banner-" + blog.Date_Published.ToString().Replace("/","").Replace(":","") + ".jpg"));
                            blog.BannerImage = directory + "Banner-"+blog.Date_Published.ToString().Replace("/", "").Replace(":", "") + ".jpg";
                        }
                    }
                }
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(blog);
            }

            
        }

        public ActionResult Article(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            blog.Views += 1;
            try
            {
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return View(blog);
            }
            catch
            {
                return View(blog);
            }
          
        }

        [HttpPost]
        public ActionResult MakeComment(FormCollection collection)
        {
            try
            {
                int blogId = Convert.ToInt32(collection.Get("blogId"));
                string commentText = collection.Get("CommentText");
                string userId = collection.Get("userId");

                Blog_Comments bComment = new Blog_Comments()
                {
                    BlogId = blogId,
                    Comment = commentText,
                    Commenter = userId,
                    InReplyTo = "No-One",
                    TimeStamp = DateTime.Now
                };
                db.Blog_Comments.Add(bComment);
                db.SaveChanges();               
                return RedirectToAction("Article", "Blogs", new { id = blogId });
            }
            catch
            {
                return RedirectToAction("Index", "Manage", null);
            }

        }

        // GET: Blogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BlogId,Title,Article,AuthorId,Date_Published,BannerImage,Featured,CommentsEnabled,Enabaled,Views")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
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
