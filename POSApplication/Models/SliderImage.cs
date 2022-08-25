namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SliderImage")]
    public partial class SliderImage
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public decimal? Price { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [StringLength(200)]
        public string ImageURL { get; set; }
    }
}
