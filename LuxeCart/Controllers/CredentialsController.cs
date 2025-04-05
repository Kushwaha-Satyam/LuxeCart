using LuxeCart.DatabaseWork.RegistrationDB;
using LuxeCart.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;


namespace LuxeCart.Controllers
{
    public class CredentialsController : Controller
    {
        private readonly UserRegistrationDB db = new UserRegistrationDB();

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserRegistration user)
        {

            if (ModelState.IsValid)
            {

                if (user.ProfilePicture == null || user.ProfilePicture.ContentLength == 0)
                {
                    ModelState.AddModelError("ProfileImageFile", "Profile image is required.");
                    return View(user);
                }

                var folderPath = Server.MapPath("~/UserImages/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Path.GetFileName(user.ProfilePicture.FileName); // Ensure only the file name is used
                var filePath = Path.Combine(folderPath, fileName);

                user.ProfilePicture.SaveAs(filePath);

                var ImageName = db.SaveProfileImage(user.ProfilePicture, filePath);

                // Assign processed image name to the model, if applicable

                user.ProfileImageUrl = ImageName;

                bool result = db.UserRegisterationMethod(user);
                if(result)
                {
                    TempData["SuccessRegister"] = "Register Successfully Login to Use more Features...";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorRegister"] = "Something Went Wrong...";
                }
               
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLogin login)
        {
            if (ModelState.IsValid)
            {
                //checking user in admins table
                var admin = db.CheckAdminEmail(login.Email);
                if (admin != null)
                {
                   if(admin.Password == login.Password)
                    {
                        var authTicket = new FormsAuthenticationTicket(
                            1,
                            admin.Email,
                            DateTime.Now,
                            DateTime.Now.AddMinutes(30),
                            login.RememberMe,
                            admin.Role
                            );

                        // Encrypt the ticket
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                        //creating cookie 
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        Response.Cookies.Add(authCookie);

                        // Redirect to the ReturnUrl or default page
                        string returnUrl = Request.QueryString["ReturnUrl"];
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl); // Redirect to the original requested URL
                        }
                        else
                        {
                            TempData["LoginSuccess"] = "Login Successful";
                            return RedirectToAction("Index", "AdminPanel"); // Default redirect
                        }
                    }
                    else
                    {

                        TempData["LoginError"] = "InValid Username Or Password...";
                        return View(login);
                    }
                }

                // If not found in Admins table, check the Users table
                var user = db.CheckUserEmail(login.Email);
                if (user != null)
                {
                    // Verify the password for user
                    if (db.VerifyPassword(login.Password, user.PasswordHash))
                    {

                        // Create authentication ticket for user
                        var authTicket = new FormsAuthenticationTicket(
                            1,
                            user.Email,
                            DateTime.Now,
                            DateTime.Now.AddMinutes(30),
                            login.RememberMe,
                            user.UserID.ToString()
                        );

                        // Encrypt the ticket
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                        // Create a cookie
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        Response.Cookies.Add(authCookie);

                        // Redirect to the ReturnUrl or default page
                        string returnUrl = Request.QueryString["ReturnUrl"];
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl); // Redirect to the original requested URL
                        }
                        else
                        {
                            TempData["LoginSuccess"] = "Login Successful";
                            return RedirectToAction("Index", "UserPanel"); // Default redirect
                        }
                    }
                    else
                    {
                        TempData["LoginError"] = "InValid Username Or Password...";
                        return View(login);
                    }
                }
                TempData["LoginError"] = "User Not Found";
            }
                
            ViewBag.ErrorLogin = "Invalid User";
            return View(login);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            TempData["Logout"] = "Logout Successful !";
            return RedirectToAction("Index", "Home");
        }
    }
}