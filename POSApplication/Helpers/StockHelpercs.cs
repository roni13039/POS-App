using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POSApplication.Helpers
{
    public class Stock
    {
        public int Id { get; set; }

       
        public string ProductName { get; set; }

        public string  ProductCategoryName { get; set; }

        public decimal PurchasePrice { get; set; }

        //public decimal SalesPrice { get; set; }

        public decimal? UpdatedPrice { get; set; }

        public int TotalQuantity { get; set; }
        public decimal? Value { get; set; }


       
    }
}