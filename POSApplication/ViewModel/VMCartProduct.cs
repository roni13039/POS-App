﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POSApplication.ViewModel
{
    public class VMCartProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}