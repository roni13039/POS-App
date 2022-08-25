using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POSApplication.ViewModel
{
    public class VMSalesMemo
    {
        //public string InvoiceNo { get; set; }




        //public DateTime Date { get; set; }

        //public string CustomerName { get; set; }

        //public string Phone { get; set; }

        public string ProductCategoryName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public int Discount { get; set; }


        public decimal SalesPrize { get; set; }

        public decimal Amount { get; set; }

       

    }
}