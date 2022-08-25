namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PurchaseInvoiceDet")]
    public partial class PurchaseInvoiceDet
    {
        public int Id { get; set; }

        public int? ProductCategoryId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal PurchasePrize { get; set; }
        public decimal Amount { get; set; }
        public int? PurchaseInvoiceMasId { get; set; }
        public virtual PurchaseInvoiceMa PurchaseInvoiceMa { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public string SerialNo { get; set; }
        public DateTime? ExpireDate { get; set; }

        public decimal? SalesPrice { get; set; }
    }
}
