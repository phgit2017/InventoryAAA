namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ProductLog
    {
        [Key]
        public long ProductLogsID { get; set; }

        public long ProductID { get; set; }

        [StringLength(32)]
        public string ProductCode { get; set; }

        [StringLength(32)]
        public string ProductDescription { get; set; }

        public decimal? Quantity { get; set; }

        public bool? IsActive { get; set; }

        public long? CategoryID { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Categories Categories { get; set; }
    }
}
