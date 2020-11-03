namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SalesOrderStatus
    {
        public SalesOrderStatus()
        {
            SalesOrders = new HashSet<SalesOrder>();
            SalesOrderStatusLogs = new HashSet<SalesOrderStatusLog>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SalesOrderStatusID { get; set; }

        [Required]
        [StringLength(16)]
        public string SalesOrderStatusName { get; set; }

        [Required]
        [StringLength(64)]
        public string SalesOrderStatusDisplay { get; set; }

        public virtual ICollection<SalesOrder> SalesOrders { get; set; }

        public virtual ICollection<SalesOrderStatusLog> SalesOrderStatusLogs { get; set; }
    }
}
