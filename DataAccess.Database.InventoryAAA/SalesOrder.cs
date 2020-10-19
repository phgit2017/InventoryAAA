namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SalesOrder
    {
        public SalesOrder()
        {
            SalesOrderDetails = new HashSet<SalesOrderDetail>();
        }

        public long SalesOrderID { get; set; }

        public string SalesNo { get; set; }

        public int SalesOrderStatusID { get; set; }

        public long CustomerID { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public int OrderTypeID { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedTime { get; set; }

        [ForeignKey("OrderTypeID")]
        public virtual OrderType OrderType { get; set; }

        [ForeignKey("SalesOrderStatusID")]
        public virtual SalesOrderStatus SalesOrderStatus { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer Customers { get; set; }

        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }
    }
}
