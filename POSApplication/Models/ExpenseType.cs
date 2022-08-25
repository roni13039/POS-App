namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExpenseType")]
    public partial class ExpenseType
    {
        public int Id { get; set; }

        
        [Required]
        [StringLength(30)]
        public string EType { get; set; }

        [Required]
        [StringLength(30)]
        public string PayOver { get; set; }
    }
}
