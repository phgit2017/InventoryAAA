namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SalesOrderStatusLogs")]
    public class SalesOrderStatusLog
    {
        [Key]
        public long SalesOrderStatusLogsID { get; set; }

        public long SalesOrderID { get; set; }

        public int SalesOrderStatusID { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        [ForeignKey("SalesOrderStatusID")]
        public virtual SalesOrderStatus SalesOrderStatus { get; set; }

        [ForeignKey("SalesOrderID")]
        public virtual SalesOrder SalesOrders { get; set; }
    }
}
