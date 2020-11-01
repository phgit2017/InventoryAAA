namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    

    [Table("ProductPricesLogs")]
    public partial class ProductPricesLog
    {
        [Key]
        public long ProductPriceLogsID { get; set; }

        public int PriceTypeID { get; set; }

        public long ProductID { get; set; }

        public decimal Price { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        [ForeignKey("PriceTypeID")]
        public virtual PriceType PriceTypes { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Products { get; set; }
    }
}
