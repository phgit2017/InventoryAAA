namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    
    [Table("CorrectionOrders")]
    public partial class CorrectionOrder
    {
        public CorrectionOrder()
        {
            CorrectionOrderDetails = new HashSet<CorrectionOrderDetail>();
        }

        public long CorrectionOrderID { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public int OrderTypeID { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedTime { get; set; }

        [ForeignKey("OrderTypeID")]
        public virtual OrderType OrderType { get; set; }

        public virtual ICollection<CorrectionOrderDetail> CorrectionOrderDetails { get; set; }
    }
}
