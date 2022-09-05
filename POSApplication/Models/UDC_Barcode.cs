namespace POSApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UDC_Barcode
    {
        public int Id { get; set; }

        public long BarcodeNumber { get; set; }
    }
}
