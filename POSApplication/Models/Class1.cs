using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POSApplication.Models
{
    public partial class sp_getAllStock_Result
    {
        public int Id { get; set; }
        public Nullable<int> ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> closingStock { get; set; }
        public Nullable<int> OpeningStock { get; set; }
        public Nullable<int> CurrentStock { get; set; }
        public decimal StockAmount { get; set; }
        public decimal TotalValue { get; set; }
    }
}