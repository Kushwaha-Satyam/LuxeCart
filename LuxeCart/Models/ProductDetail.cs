using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuxeCart.Models
{
    public class ProductDetail
    {
        //public int ProductID { get; set; } // Unique Identifier
        public string ProductName { get; set; } // Name of the product
        public string ProductCompany { get; set; } // Name of Company
        public string ProductDescription { get; set; } // Product description
        public decimal ProductPrice { get; set; } // Price of the product
        public int ProductQuantity { get; set; } // Available stock quantity
        public string ProductImageUrl { get; set; } // Image path for the product

        public HttpPostedFileBase ProductImageFile { get; set; }
        public int CategoryID { get; set; } // Foreign key to Category table

        // Navigation property to Category (Foreign Key Relationship)
        public virtual Category Category { get; set; }
        public DateTime CreatedDate { get; set; } // Date when product was added
        public bool IsAvailable { get; set; } // Availability status
    }
}