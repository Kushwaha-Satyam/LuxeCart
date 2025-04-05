using LuxeCart.AutherizationFilter;
using LuxeCart.DatabaseWork.AdminPanelDB;
using LuxeCart.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LuxeCart.Controllers
{
    [RoleAuthorize("Admin")]
    public class AdminPanelController : Controller
    {
        private ProductRepository productRepo;
        private CategoryRepository categoryRepo;

        public AdminPanelController()
        {
            productRepo = new ProductRepository();
            categoryRepo = new CategoryRepository();
        }
        public ActionResult Index()
        {

            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                ViewBag.UserRole = authTicket.UserData; // Role is stored in UserData
            }

            return View();
        }
        [HttpGet]
        public ActionResult AddProduct()
        {
            var categories = categoryRepo.GetAllCategories() ?? new List<Category>();
            ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName");

            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(ProductDetail data)
        {
            if (ModelState.IsValid)
            {
                if (data.ProductImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(data.ProductImageFile.FileName);
                    string extension = Path.GetExtension(data.ProductImageFile.FileName);
                    fileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    string filePath = "/ProductImages/" + fileName;
                    data.ProductImageFile.SaveAs(Server.MapPath("~" + filePath));
                    data.ProductImageUrl = filePath;
                }

                bool result = productRepo.AddProduct(data); // Add product using repository
                if (result)
                {
                    ViewBag.SuccessMessage = "Product added successfully!";
                    ModelState.Clear();
                    return View();
                }
               
            }

            return View(data);
        }

        public ActionResult AllProducts()
        {
           var data = new List<Product>();
            data = productRepo.AllProducts();
            return View(data);
        }
        public ActionResult ProductDetail(int ProductID)
        {
            var product = productRepo.GetProduct(ProductID);

            if (product == null)
            {
                return HttpNotFound(); 
            }

            return View(product); 
        }
        [HttpGet]
        public ActionResult EditProduct(int ProductID)
        {
            var product = productRepo.GetProduct(ProductID);
            if (product == null)
            {
                return HttpNotFound();
            }
            var categories = categoryRepo.GetAllCategories() ?? new List<Category>();
            ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName");
            return View(product);
        }
        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool updateSuccessful =productRepo.UpdateProduct(product);

                    if (updateSuccessful)
                    {
                        TempData["UpdateSuccess"] = "Updated Successfully";
                        return RedirectToAction("ALlProducts"); 
                    }
                    else
                    {
                        ModelState.AddModelError("", "There was an error updating the product.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error: " + ex.Message);
                }
            }
            return View(product);
        } 

        public ActionResult DeleteProduct(int ProductID)
        {
            bool isDelete = productRepo.DeleteProduct(ProductID);

            if (isDelete)
            {
                TempData["DeleteSuccess"] = "Deleted Successfully";
                return RedirectToAction("AllProducts");
            }
            else
            {
                TempData["DeleteError"] = "Something Went Wrong";
                return View("AllProducts");
            }
        }
    }
}