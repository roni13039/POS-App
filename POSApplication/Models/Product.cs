namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {    [Key]
        public int Id { get; set; }

        [StringLength(30)]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(30)]
        public string ProductName { get; set; }

       
        [ForeignKey("ProductCategory")]
        public int? ProductCategoryId { get; set; }

        [StringLength(200)]
        public string ImageURL { get; set; }

     
        public virtual ProductCategory ProductCategory { get; set; }
    }
}
