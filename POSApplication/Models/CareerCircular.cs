namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CareerCircular")]
    public partial class CareerCircular
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string JobTitle { get; set; }

        [StringLength(100)]
        public string ImageURL { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        //[DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PublishedDate { get; set; }


        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ExpiredDate { get; set; }
    }
}
