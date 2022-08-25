namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("PurchaseInvoiceMas")]
    public partial class PurchaseInvoiceMa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseInvoiceMa()
        {
            PurchaseInvoiceDets = new HashSet<PurchaseInvoiceDet>();
        }

        public int Id { get; set; }

        public int? SupplierId { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Secu_User")]
        
        public int? UserId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseInvoiceDet> PurchaseInvoiceDets { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Secu_User Secu_User { get; set; }
    }
}
