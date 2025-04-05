using LuxeCart.AutherizationFilter;
using LuxeCart.DatabaseWork.AdminPanelDB;
using LuxeCart.DatabaseWork.UserPanelDB;
using LuxeCart.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuxeCart.Controllers
{
    [RoleAuthorize("User")]
    public class ProductController : Controller
    {

        private readonly ProductRepository productRepo;

        private readonly LuxeCartEntities dbContext;
        public ProductController()
        {
            productRepo = new ProductRepository();
            dbContext = new LuxeCartEntities();
        }
        public ProductController(ProductRepository productRepo)
        {
            this.productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
        }

        [AllowAnonymous]
        public ActionResult ProductDetail(int id)
        {
            var product = productRepo.GetProduct(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
        public ActionResult AddToCart(int productID, int userID)
        {
            var product = productRepo.GetProduct(productID); 

            if (product == null)
            {
                return HttpNotFound("Product not found.");
            }

            // Get or create the user's cart
            var cart = dbContext.Carts
             .Include(c => c.CartItems ) 
             .FirstOrDefault(c => c.UserID == userID); // Correct: Lambda expression

            if (cart == null)
            {
                cart = new Cart { UserID = userID, CreatedAt = DateTime.UtcNow };
                dbContext.Carts.Add(cart);
            }

            // Check if the product is already in the cart
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId ==productID );
            if (cartItem != null)
            {
                // Update quantity if the product is already in the cart
                cartItem.Quantity++;
            }
            else
            {
                // Add new product to the cart
                cartItem = new CartItem { ProductId = productID, Quantity = 1 };
                cart.CartItems.Add(cartItem);
            }

            // Save changes to the database
            int i = dbContext.SaveChanges();
            if (i > 0)
            {
                TempData["AddedToCart"] = " Added To Cart. ";
                return RedirectToAction("ViewCart", new { userId = userID });
            }
            return View();
            
        }
        public ActionResult ViewCart(int userId)
        {
            // Retrieve the user's cart with items and products
            var cart = dbContext.Carts
                .Include(c => c.CartItems.Select(ci => ci.Product)) 
                .FirstOrDefault(c => c.UserID == userId); 

            if (cart == null)
            {
                return View("EmptyCart");
            }

            return View(cart);
        }

        //removing product from cart
        public ActionResult RemoveFromCart(int productID, int userID)
        {
            // Retrieve the user's cart

            var cart = dbContext.Carts
                .Include(c => c.CartItems) 
                .FirstOrDefault(c => c.UserID == userID);

            if (cart == null)
            {
                return HttpNotFound("Cart not found.");
            }

            // Find the cart item to remove
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productID);
            if (cartItem == null)
            {
                return HttpNotFound("Product not found in the cart.");
            }

            dbContext.CartItems.Remove(cartItem);
            dbContext.SaveChanges();

            TempData["RemoveFromCartMessage"] = "Product removed from cart successfully.";
            return RedirectToAction("ViewCart", new { userId = userID });
        }

        //showing products  by categories
        public ActionResult ProductByCategory(string category)
        {
            var categoryMap = new Dictionary<string, int>
        {
            { "Electronics", 1 },
            { "Clothing", 2 },
            { "HomeAndLiving", 3 }
        };

            if (!categoryMap.ContainsKey(category))
            {
                return RedirectToAction("Index", "UserPanel");
            }

            int categoryId = categoryMap[category];
            var products = productRepo.AllProducts()
                                      .Where(p => p.CategoryID == categoryId)
                                      .ToList();

            ViewBag.Category = category;
            return View(products);
        }
    }
}