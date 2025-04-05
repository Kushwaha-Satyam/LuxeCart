using LuxeCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuxeCart.DatabaseWork.AdminPanelDB
{
    public class ProductRepository
    {
        private readonly LuxeCartEntities dbContext;
        public ProductRepository()
        {
            dbContext = new LuxeCartEntities();
        }
        public bool AddProduct(ProductDetail data)
        {
            var insertData = new Product()
            {
                ProductName = data.ProductName,
                ProductCompany = data.ProductCompany,
                ProductDescription = data.ProductDescription,
                ProductPrice = data.ProductPrice,
                ProductQuantity = data.ProductQuantity,
                ProductImageUrl = data.ProductImageUrl,
                Category = data.Category,
                CategoryID = data.CategoryID,
                CreatedDate = DateTime.Now,
                IsAvailable = data.IsAvailable,
            };

            dbContext.Products.Add(insertData);
            var result = dbContext.SaveChanges();
            return result > 0;
        }

        public List<Product> AllProducts()
        {
            return dbContext.Products.ToList();
        }

        public Product GetProduct(int id)
        {
            return dbContext.Products.SingleOrDefault(p => p.ProductID == id);
        }

        public bool UpdateProduct(Product product)
        {
            try
            {
                var existingProduct = dbContext.Products.SingleOrDefault(p => p.ProductID == product.ProductID);

                if (existingProduct == null)
                {
                    return false;
                }

                existingProduct.ProductName = product.ProductName;
                existingProduct.ProductCompany = product.ProductCompany;
                existingProduct.ProductDescription = product.ProductDescription;
                existingProduct.ProductPrice = product.ProductPrice;
                existingProduct.ProductQuantity = product.ProductQuantity;
                existingProduct.CategoryID = product.CategoryID;
                existingProduct.IsAvailable = product.IsAvailable;
                existingProduct.CreatedDate = product.CreatedDate;

                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool DeleteProduct(int id)
        {
            try
            {
                var product = dbContext.Products.SingleOrDefault(p => p.ProductID == id);

                if (product == null)
                {
                    return false;
                }

                dbContext.Products.Remove(product);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}