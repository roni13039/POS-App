namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderMas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderMas()
        {
            OrderDets = new HashSet<OrderDet>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(55)]
        public string CustomerName { get; set; }

        [StringLength(55)]
        public string Town { get; set; }

        [Required]
        [StringLength(55)]
        public string Phone { get; set; }

        [StringLength(55)]
        public string Email { get; set; }
         public DateTime AddedOn { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDet> OrderDets { get; set; }
    }
}
