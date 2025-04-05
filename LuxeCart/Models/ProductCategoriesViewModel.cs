using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuxeCart.Models
{
    public class ProductCategoriesViewModel
    {
        public List<Product> Electronics { get; set; }
        public List<Product> Clothes { get; set; }
        public List<Product> HomeAndLiving { get; set; }
        public string ActiveFilter { get; set; } // Track the active filter
    }
}