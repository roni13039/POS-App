using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POSApplication.ViewModel
{
    public class VMPurchaseInvoice
    {
        public DateTime Date { get; set; }

        public string ProductName { get; set; }

        public string SupplierName { get; set; }

        public int Quantity { get; set; }

        public decimal? PurchasePrize { get; set; }

        public decimal? Value { get; set; }

        public string ProductCategoryName { get; set; }
        public DateTime? ExpireDate { get; internal set; }
    }
}