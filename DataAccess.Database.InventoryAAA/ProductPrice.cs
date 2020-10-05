namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProductPrices")]
    public partial class ProductPrice
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PriceTypeID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ProductID { get; set; }

        public decimal Price { get; set; }

        [ForeignKey("PriceTypeID")]
        public virtual PriceType PriceTypes { get; set; }
        
        [ForeignKey("ProductID")]
        public virtual Product Products { get; set; }
    }
}
