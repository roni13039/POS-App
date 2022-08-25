namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CompanyInformation")]
    public partial class CompanyInformation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(80)]
        public string Address { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(40)]
        public string Description { get; set; }
        public string CompanyImage { get; set; }
        
    }
}
