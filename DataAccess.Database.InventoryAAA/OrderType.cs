namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderType
    {
        public OrderType()
        {
            PurchaseOrders = new HashSet<PurchaseOrder>();
            SalesOrders = new HashSet<SalesOrder>();
            CorrectionOrders = new HashSet<CorrectionOrder>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderTypeID { get; set; }

        [Required]
        [StringLength(16)]
        public string OrderTypeName { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }

        public virtual ICollection<SalesOrder> SalesOrders { get; set; }

        public virtual ICollection<CorrectionOrder> CorrectionOrders { get; set; }
    }

}
