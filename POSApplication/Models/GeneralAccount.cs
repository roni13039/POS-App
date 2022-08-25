namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GeneralAccount
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey("ExpenseType")]
        public int? ExpenseTypeId { get; set; }

        [StringLength(30)]
        public string PayOver { get; set; }

        public decimal CashPayment { get; set; }

        [StringLength(20)]
        public string PayTo { get; set; }

        public virtual ExpenseType ExpenseType { get; set; }
    }
}
