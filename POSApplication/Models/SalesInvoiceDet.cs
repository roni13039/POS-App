namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalesInvoiceDet")]
    public partial class SalesInvoiceDet
    {
        public int Id { get; set; }
        [ForeignKey("SalesInvoiceMas")]
        public int? SalesInvoiceMasId { get; set; }

        public int? ProductCategoryId { get; set; }

        public int? ProductId { get; set; }

        public decimal PurchasePrize { get; set; }

        public decimal SalesPrize { get; set; }

        public decimal? Amount { get; set; }

        public int? Quantity { get; set; }

        public int? Discount { get; set; }

        public int? PriceType { get; set; }

        public decimal? UpdatedPrize { get; set; }

        public virtual Product Product { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }

        public virtual SalesInvoiceMas SalesInvoiceMas { get; set; }
    }
}
