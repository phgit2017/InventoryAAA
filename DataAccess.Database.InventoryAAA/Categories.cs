namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Categories
    {
        public Categories()
        {
            Products= new HashSet<Product>();
            ProductLogs = new HashSet<ProductLog>();
        }

        [Key]
        public long CategoryID { get; set; }

        [Required]
        [StringLength(128)]
        public string CategoryName { get; set; }

        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedTime { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<ProductLog> ProductLogs { get; set; }

    }
}
