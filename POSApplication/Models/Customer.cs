namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Customer")]
    public partial class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string CustomerName { get; set; }

        [StringLength(50)]
        public string Hint { get; set; }

        [StringLength(80)]
        public string Address { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        public decimal? OpeningBalance { get; set; }

        public decimal? TotalPurchase { get; set; }

        public decimal? TotalPaid { get; set; }

        public decimal? DueBalance { get; set; }
    }
}
