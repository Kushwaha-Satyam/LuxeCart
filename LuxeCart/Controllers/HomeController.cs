using LuxeCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LuxeCart.Controllers
{
    public class HomeController : Controller
    {
        private readonly LuxeCartEntities dbContext;

        public HomeController()
        {
            dbContext = new LuxeCartEntities();
        }

        public HomeController(LuxeCartEntities dbContext)
        {
            this.dbContext = dbContext;
        }

        public ActionResult Index()
        {
            var products = dbContext.Products
                .OrderByDescending(p => p.ProductID)
                .Take(10)
                .ToList();

            return View(products);
        }

        public ActionResult Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                ViewBag.Message = "Please enter a search term.";
                return View(new List<Product>());
            }

            try
            {
                var products = SearchProducts(searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View(products);
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                ViewBag.ErrorMessage = "An error occurred while searching for products.";
                return View(new List<Product>());
            }
        }

        public List<Product> SearchProducts(string searchTerm)
        {
            // Ensure the search term is not null or empty
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<Product>();
            }

            // Perform the search directly in the database query
            var filteredProducts = dbContext.Products
             .Where(p => (p.ProductName != null && p.ProductName.Contains(searchTerm)) ||
                    (p.ProductDescription != null && p.ProductDescription.Contains(searchTerm)))
             .ToList();

            return filteredProducts;
        }
    }
}
