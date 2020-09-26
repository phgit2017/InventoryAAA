namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CustomerStatus
    {
        public CustomerStatus()
        {
            Customers = new HashSet<Customer>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerStatusID { get; set; }

        [Required]
        [StringLength(32)]
        public string CustomerStatusName { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
