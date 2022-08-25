namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ClientsFeedback")]
    public partial class ClientsFeedback
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ClientsName { get; set; }

        [StringLength(200)]
        public string ImageURL { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }
    }
}
