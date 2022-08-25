using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POSApplication.ViewModel
{
    public class VmPurchaseInvoice
    {

        public DateTime Date { get; set; }
        public string ProductCategory { get; set; }

        public string Product { get; set; }

        public decimal PurchasePrize { get; set; }

        public decimal? SalesPrize { get; set; }

        public int Quantity { get; set; }

        public int? Discount { get; set; }
      

    }
}