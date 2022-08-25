namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderDet")]
    public partial class OrderDet
    {
        public int Id { get; set; }

        public int? ProductId { get; set; }

        public decimal? Price { get; set; }

        public int? Qunatity { get; set; }

        public int? OrderMasId { get; set; }


        public virtual Product Product { get; set; }
        public virtual OrderMas OrderMas { get; set; }
    }
}
