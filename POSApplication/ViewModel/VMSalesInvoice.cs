using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POSApplication.ViewModel
{
    public class VMSalesInvoice
    {

        public DateTime? Date { get; set; }
        public string ProductName { get; set; }

        public string CustomerName { get; set; }

        public int Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal Value { get; set; }

        public string ProductCategoryName { get; set; }
        public string Phone { get; internal set; }
    }
}