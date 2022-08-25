using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POSApplication.ViewModel
{
    public class VMCustomer
    {
        
        public string CustomerName { get; set; }

     
      
      
        public string Phone { get; set; }

       
        public decimal TotalPurchase { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal DueBalance { get; set; }


    }
}