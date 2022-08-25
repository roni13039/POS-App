namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("SalesInvoiceMas")]
    public partial class SalesInvoiceMas
    {
        public int Id { get; set; }

        [StringLength(80)]
        public string InvoiceNo { get; set; }
      
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        public int? CustomerId { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }
        public string CustomerName { get; set; }
        
        [StringLength(80)]
        public string Address { get; set; }

        [StringLength(20)]
        public string Description { get; set; }
        public decimal? TotalBill { get; set; }

        [ForeignKey("Secu_User")]
        public int? UserId { get; set; }
        public int? Year { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        public decimal? PaidBill { get; set; }
        public decimal? DueBalance { get; set; }
        

        public virtual Customer Customer { get; set; }
        public virtual Secu_User Secu_User { get; set; }

        


    }
}
