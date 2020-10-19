namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Customer
    {
        public Customer()
        {
            SalesOrders = new HashSet<SalesOrder>();
        }

        [Key]
        public long CustomerID { get; set; }

        [StringLength(32)]
        public string CustomerCode { get; set; }

        [StringLength(128)]
        public string FirstName { get; set; }

        [StringLength(128)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string FullAddress { get; set; }

        public int CustomerStatusID { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedTime { get; set; }

        [ForeignKey("CustomerStatusID")]
        public CustomerStatus CustomerStatus { get; set; }

        public virtual ICollection<SalesOrder> SalesOrders { get; set; }
    }
}
