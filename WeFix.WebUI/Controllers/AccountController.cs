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
using System.Data.Entity.Core.Objects;
using System.Web.Security;
using WeFix.Logic;

namespace WeFix.WebUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private SignInManager _signInManager;
        private UserManager _userManager;
        private EFDbContext db = new EFDbContext();
        private const string ExpectedImagePrefixJpeg = "data:image/jpeg;base64,";
        private const string ExpectedImagePrefixPng = "data:image/png;base64,";
        public AccountController()
        {
        }

        public AccountController(UserManager userManager, SignInManager signInManager)
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
        [AllowAnonymous]
        public ActionResult SignUp(int? id)
        {
            if (id == 0)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Manage");
                }
                ViewBag.CountryId = new SelectList(db.Countries, "CountryAccessCode", "CountryName", "+27");
                return View();
            }
            else
            {
                if (id == 1)
                {
                    ViewBag.ErrorMessage = "Passwords do not match!";
                }
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Manage");
                }
                ViewBag.CountryId = new SelectList(db.Countries, "CountryAccessCode", "CountryName", "+27");
                return View();
            }
           
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(SignUpViewModel model, FormCollection collection)
        {           
            try
            {
                if (model.Password == model.ConfirmPassword)
                {
                    if (model.IsManager == true)
                    {
                        string code = collection.Get("CountryCode");
                        string cell = string.Empty;
                        if (model.Cell.StartsWith("0"))
                        {
                            cell = code + model.Cell.Substring(0, 1);
                        }
                        else
                        {
                            cell = code + model.Cell;
                        }
                        
                        Organisation newOrganisation;
                        OrganisationManager orgManager;
                        var orguser = new User
                        {
                            UserName = model.Email,
                            Email = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Cell = cell,
                            PhoneNumber = cell,
                            IsManager = model.IsManager,
                            EmailConfirmed = true,
                            ConnectionId = "",
                            IsOnline = false,
                            LastOnline = DateTime.Now
                        };
                        if (model.Password != model.ConfirmPassword)
                        {
                            return View(model);
                        }
                        else
                        {
                            var orgResult = await UserManager.CreateAsync(orguser, model.Password);
                            if (orgResult.Succeeded)
                            {
                                int orgId = 0;
                                newOrganisation = new Organisation() { Approved = false, Name = model.OrganisationName };
                                db.Organisations.Add(newOrganisation);
                                db.SaveChanges();
                                while (orgId == 0)
                                {
                                    orgId = newOrganisation.OrganisationId;
                                }
                                orgManager = new OrganisationManager() { Position = "Organisation Manager", UserId = orguser.Id, OrganisationId = orgId };
                                db.OrganisationManagers.Add(orgManager);
                                db.SaveChanges();
                                Email EmailFault = new Email();
                                EmailFault.ProcessOrganisation(newOrganisation, db);
                                string data_uri = collection.Get("base64");
                                string newUserId = string.Empty;
                                while (newUserId == string.Empty)
                                {
                                    newUserId = (from x in db.Users where x.Id == orguser.Id select x.Id).FirstOrDefault();
                                }
                                User newUser = (from x in db.Users where x.Id == orguser.Id select x).FirstOrDefault();
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
                                    string directory = "~/Content/uploads/profiles/" + newUser.Id + "/";
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
                                db.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                                await SignInManager.SignInAsync(newUser, isPersistent: false, rememberBrowser: false);
                                return RedirectToAction("Index", "Manage");
                            }
                        }
                    }
                    else
                    {
                        string code = collection.Get("CountryCode");
                        string cell = string.Empty;
                        if (model.Cell.StartsWith("0"))
                        {
                            cell = code + model.Cell.Substring(0, 1);
                        }
                        else
                        {
                            cell = code + model.Cell;
                        }

                        var pUser = new User { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Cell = cell, PhoneNumber = cell, IsManager = model.IsManager, EmailConfirmed = true };
                        var result = await UserManager.CreateAsync(pUser, model.Password);
                        if (result.Succeeded)
                        {

                            try
                            {
                                PublicUser publicUser = new PublicUser() { UserId = pUser.Id };
                                db.PublicUsers.Add(publicUser);
                                db.SaveChanges();
                                string data_uri = collection.Get("base64");
                                string newUserId = string.Empty;
                                while (newUserId == string.Empty)
                                {
                                    newUserId = (from x in db.Users where x.Id == pUser.Id select x.Id).FirstOrDefault();
                                }
                                User newUser = (from x in db.Users where x.Id == pUser.Id select x).FirstOrDefault();

                                if (data_uri != null)
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
                                    string directory = "~/Content/uploads/profiles/" + newUser.Id + "/";
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

                                    db.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    await SignInManager.SignInAsync(pUser, isPersistent: false, rememberBrowser: false);
                                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                                    // Send an email with this link
                                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                                    return RedirectToAction("Index", "Manage");
                                }
                            }
                            catch (Exception ex)
                            {
                                ViewBag.ErrorMessage = ex.Message;
                            }
                        }
                    }
                    
                }
                else
                {
                    return RedirectToAction("SignUp", "Account", new { id = 1 });
                }
            }
            catch
            {
                return RedirectToAction("SignUp", "Account", null);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #region Other
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage", null);
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = (from x in db.Users where x.Email == model.Email select x).FirstOrDefault();
            DepartmentManager dm = null;
            dm = (from x in db.DepartmentManagers where x.UserId == user.Id select x).FirstOrDefault();

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (dm != null)
                    {
                        if(dm.CountPasswordChanges==0)
                        {
                            return RedirectToAction("ResetPassword", "Account", null);
                        }
                        else
                        {
                            return RedirectToAction("Index","Manage",null);
                        }
                    }
                    return RedirectToAction("Index", "Manage", null);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        #region Unconcered
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register        
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        #endregion
        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model, FormCollection collection)
        {
            var email = collection.Get("email");
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                User newUser = (from x in db.Users where x.UserName == model.Email select x).FirstOrDefault();
                DepartmentManager dm = (from x in db.DepartmentManagers where x.UserId == user.Id select x).FirstOrDefault();
                dm.CountPasswordChanges += 1;
                db.Entry(dm).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }
        #endregion
        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl, FormCollection collection)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                if (model.IsManager)
                {
                    Organisation newOrganisation;
                    OrganisationManager orgManager;
                    string code = collection.Get("OrgCountryCode");
                    string cell = code + model.Cell;
                    var orguser = new User
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Cell = cell,
                        PhoneNumber = cell,
                        IsManager = model.IsManager,
                        UserPhotoUrl = model.UserPhoto,
                        EmailConfirmed = true,
                        ConnectionId = "",
                        LastOnline = DateTime.Now,
                        IsOnline = false
                    };
                    var orgresult = await UserManager.CreateAsync(orguser);
                    if (orgresult.Succeeded)
                    {
                        orgresult = await UserManager.AddLoginAsync(orguser.Id, info.Login);
                        if (orgresult.Succeeded)
                        {
                            int orgId = 0;
                            newOrganisation = new Organisation() { Approved = false, Name = model.OrganisationName };
                            db.Organisations.Add(newOrganisation);
                            db.SaveChanges();
                            while (orgId == 0)
                            {
                                orgId = newOrganisation.OrganisationId;
                            }
                            orgManager = new OrganisationManager() { Position = "Organisation Manager", UserId = orguser.Id, OrganisationId = orgId };
                            db.OrganisationManagers.Add(orgManager);
                            db.SaveChanges();
                            Email EmailFault = new Email();
                            EmailFault.ProcessOrganisation(newOrganisation, db);
                            string data_uri = collection.Get("orgbase64");
                            string newUserId = string.Empty;
                            while (newUserId == string.Empty)
                            {
                                newUserId = (from x in db.Users where x.Id == orguser.Id select x.Id).FirstOrDefault();
                            }
                            User newUser = (from x in db.Users where x.Id == orguser.Id select x).FirstOrDefault();
                            if (data_uri != null)
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
                                string directory = "~/Content/uploads/profiles/" + newUser.Id + "/";
                                Image image;
                                using (MemoryStream ms = new MemoryStream(bytes))
                                {
                                    image = Image.FromStream(ms);
                                    if (Directory.Exists(directory))
                                    {
                                        directory.Substring(directory.Length - 1);
                                        image.Save(Server.MapPath(directory + ".jpg"));
                                        newUser.UserPhotoUrl = directory + ".jpg";
                                    }
                                    else
                                    {
                                        Directory.CreateDirectory(Server.MapPath(directory));
                                        directory.Substring(directory.Length - 1);
                                        image.Save(Server.MapPath(directory + ".jpg"));
                                        newUser.UserPhotoUrl = directory + ".jpg";
                                    }
                                }
                            }

                            db.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            await SignInManager.SignInAsync(orguser, isPersistent: false, rememberBrowser: false);
                            return RedirectToAction("Index", "Manage");
                        }
                    }
                }
                else
                {
                    string code = collection.Get("CountryCode");
                    string cell = code + model.Cell;
                    var pUser = new User { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Cell = cell, PhoneNumber=cell, IsManager = model.IsManager, EmailConfirmed = true };
                    var result = await UserManager.CreateAsync(pUser);
                    if (result.Succeeded)
                    {
                        result = await UserManager.AddLoginAsync(pUser.Id, info.Login);
                        if(result.Succeeded)
                        {
                            PublicUser publicUser = new PublicUser() { UserId = pUser.Id };
                            db.PublicUsers.Add(publicUser);
                            db.SaveChanges();
                            string data_uri = collection.Get("base64");
                            string newUserId = string.Empty;
                            while (newUserId == string.Empty)
                            {
                                newUserId = (from x in db.Users where x.Id == pUser.Id select x.Id).FirstOrDefault();
                            }
                            User newUser = (from x in db.Users where x.Id == pUser.Id select x).FirstOrDefault();

                            if (data_uri != null)
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
                                string directory = "~/Content/uploads/profiles/" + newUser.Id + "/";
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
                            db.Entry(newUser).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            await SignInManager.SignInAsync(pUser, isPersistent: false, rememberBrowser: false);
                            return RedirectToAction("Index", "Manage");
                        }
                    }
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateProfile(string Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(Id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }



        public ActionResult NotificationOne()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}