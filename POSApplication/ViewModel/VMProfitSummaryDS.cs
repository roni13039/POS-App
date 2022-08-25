using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POSApplication.ViewModel
{
    public class VMProfitSummaryDS
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public decimal? Profit { get; set; }
        public decimal? Expense { get; set; }
        public decimal? TotalProfit { get; set; }
    }
}