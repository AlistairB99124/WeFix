using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class HomeController : Controller
    {
        Domain.Context.EFDbContext db = new Domain.Context.EFDbContext();

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage", null);
            }
            return View();
        }

        public ActionResult Hack()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Contact(int? id)
        {
            if (id == null)
            {
                return View();
            }
            else if (id==0)
            {
                ViewBag.Result = "Thank you, message sent!";
                return View();
            }
            else
            {
                ViewBag.Result = "Failure, email was not sent";
                return View();
            }
           
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult TroubleShooting()
        {
            Helpers helpers = new Logic.Helpers();
            var suburb = helpers.GetSuburb(-34.0394881723299, 18.4341033478852);
            return View();
        }

        [HttpGet]
        public ActionResult Search(string query)
        {
            if (query == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }           
            string[] splits = query.Split(' ');
            List<string> keywords = splits.ToList();
            List<Blog> blogResults = new List<Blog>();
            List<Fault> faultResults = new List<Fault>();
            List<User> userResults = new List<User>();

            foreach (string key in keywords)
            {
                var a = (from x in db.Blogs where x.Article.Contains(key) || x.Date_Published.ToString().Contains(key) || x.Title.Contains(key) select x).FirstOrDefault();

                if (a != null)
                {
                    blogResults.Add(a);
                }

                var b = (from x in db.Faults where x.DateCreated.ToString().Contains(key) || x.Description.Contains(key) select x).FirstOrDefault();
                if (b != null)
                {
                    faultResults.Add(b);
                }

                var c = (from x in db.Users where x.FirstName.Contains(key) || x.LastName.Contains(key) select x).FirstOrDefault();
                if (c != null)
                {
                    userResults.Add(c);
                }
            }
            ViewBag.BlogResults = blogResults;
            ViewBag.FaultResults = faultResults;
            ViewBag.UserResults = userResults;
            ViewBag.Query = query;
            return View();
        }

        [HttpPost]
        public ActionResult Search(FormCollection collection)
        {
            string q = collection.Get("q");          
            
            return RedirectToAction("Search", "Home", new{ query=q});
        }

        [HttpPost]
        public ActionResult Contact(FormCollection collection)
        {
            try
            {
                string from = collection.Get("Email");
                string subject = collection.Get("Subject");
                string body = collection.Get("Body");

                var helper = new Email();
                helper.ProcessContact(from, subject, body);
                return RedirectToAction("Contact", "Home", new { id = 0 });
            }
            catch
            {
                return RedirectToAction("Contact","Home",new { id=1});
            }
           
        }

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult Error500()
        {
            return View();
        }


        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChatRoom(string query)
        {
            if (query == null)
            {
                return RedirectToAction("Error500", "Home", null);
            }
            var fromUser = db.Users.Find(query);
            if (fromUser == null)
            {
                return RedirectToAction("Error400", "Home", null);
            }
            var toUserId = HttpContext.User.Identity.GetUserId();
            var toUser = db.Users.Find(toUserId);
            var unOrderedMessages = new List<ChatMessage>();
            var fromMessages = (from x in db.ChatMessages where x.fromUserId == query && x.toUserId == toUserId select x).ToList();
            var toMessages = (from x in db.ChatMessages where x.fromUserId == toUserId && x.toUserId == query select x).ToList();
            unOrderedMessages.AddRange(fromMessages);
            unOrderedMessages.AddRange(toMessages);
            var messages = (from x in unOrderedMessages orderby x.Time_Stamp descending select x).ToList();
            foreach(var m in fromMessages)
            {
                var mess = db.ChatMessages.Find(m.ChatMessageId);
                mess.Viewed = true;
                db.Entry(mess).State = System.Data.Entity.EntityState.Modified;                
            }
            db.SaveChanges();
            ViewBag.FromUser = fromUser;
            ViewBag.ToUser = toUser;

            return View(messages);
        }

        [HttpPost]
        public ActionResult ChatRoom(FormCollection collection)
        {
            var body = collection.Get("q");
            var fromUserId = collection.Get("FromUser");
            var toUserId = collection.Get("ToUser");
            var Chat = new ChatMessage()
            {
                Body = body,
                fromUserId = fromUserId,
                toUserId = toUserId,
                Time_Stamp = DateTime.Now
            };
            db.ChatMessages.Add(Chat);
            db.SaveChanges();
            return RedirectToAction("ChatRoom", "Home", new { query = fromUserId });
        }
    }
}