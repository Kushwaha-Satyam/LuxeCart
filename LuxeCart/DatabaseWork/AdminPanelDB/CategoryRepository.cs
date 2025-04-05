using LuxeCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuxeCart.DatabaseWork.AdminPanelDB
{
    public class CategoryRepository
    {
        private LuxeCartEntities dbContext;

        public CategoryRepository()
        {
            dbContext = new LuxeCartEntities();
        }

        // Get all categories
        public List<Category> GetAllCategories()
        {
            return dbContext.Categories.ToList() ?? new List<Category>();
        }
    }
}