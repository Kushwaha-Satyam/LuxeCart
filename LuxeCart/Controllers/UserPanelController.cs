using LuxeCart.AutherizationFilter;
using LuxeCart.DatabaseWork.RegistrationDB;
using LuxeCart.DatabaseWork.UserPanelDB;
using LuxeCart.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LuxeCart.Controllers
{
    [RoleAuthorize("User")]
    public class UserPanelController : Controller
    {
        private readonly UserRegistrationDB db = new UserRegistrationDB();

        private readonly LuxeCartEntities dbContext;

        private UserDb UserRepo;
        public UserPanelController()
        {
            UserRepo = new UserDb();
            dbContext = new LuxeCartEntities();
        }
       
        public ActionResult Index(string filter = "All")
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                Session["UserID"] = authTicket.UserData; 
            }

            // Fetch all products from the database
            var products = dbContext.Products.AsQueryable();

            // Apply filters
            switch (filter)
            {
                case "New Arrivals":
                    products = products.OrderByDescending(p => p.CreatedDate).Take(10);
                    break;
                case "Best Sellers":
                    products = products.OrderByDescending(p => p.ProductPrice).Take(10);
                    break;
                default:
                    // Show all products
                    break;
            }

            // Categorize products
            var electronics = products.Where(p => p.CategoryID == 1).ToList();
            var clothes = products.Where(p => p.CategoryID == 2).ToList();
            var homeAndLiving = products.Where(p => p.CategoryID == 3).ToList();

            // Pass categorized products to the view
            var model = new ProductCategoriesViewModel
            {
                Electronics = electronics,
                Clothes = clothes,
                HomeAndLiving = homeAndLiving,
                ActiveFilter = filter
            };

            return View(model);

        }

        public ActionResult UserDetail(int id)
        {
            var user = UserRepo.GetUser(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = UserRepo.GetUser(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }
        
        [HttpPost]
        public ActionResult Edit(RegisterUser user , HttpPostedFileBase ProfilePicture)
        {
            if (ModelState.IsValid)
            {
                if (ProfilePicture == null || ProfilePicture.ContentLength == 0)
                {
                    ModelState.AddModelError("ProfileImageFile", "Profile image is required.");
                    return View(user);
                }

                var folderPath = Server.MapPath("~/UserImages/");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Path.GetFileName(ProfilePicture.FileName); // Ensure only the file name is used
                var filePath = Path.Combine(folderPath, fileName);

                ProfilePicture.SaveAs(filePath);

                var ImageName = db.SaveProfileImage(ProfilePicture, filePath);

                // Assign processed image name to the model, if applicable

                user.ProfileImageUrl = ImageName;

                bool result = db.UserUpdateMethod(user);
                if (result)
                {
                    TempData["SuccessUpdate"] = "Register Successfully Login to Use more Features...";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorUpdate"] = "Something Went Wrong...";
                }

            }
            return View();
        }


        // deleting account
        [HttpPost]
        public ActionResult DeleteUser(int UserID)
        {
            var result = db.DeleteUser(UserID);
            if (result)
            {
                TempData["SuccessDelete"] = "Account deleted successfully.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while deleting account.";
                return RedirectToAction("Index");
            }
        }


    }

}