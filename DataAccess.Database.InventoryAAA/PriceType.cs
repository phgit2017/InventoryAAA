namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PriceTypes")]
    public partial class PriceType
    {
        public PriceType()
        {
            ProductPrices = new HashSet<ProductPrice>();
            SalesOrderDetails = new HashSet<SalesOrderDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PriceTypeID { get; set; }

        [Required]
        [StringLength(32)]
        public string PriceTypeName { get; set; }
        
        //[InverseProperty("PriceType_PriceTypeID")]
        public virtual ICollection<ProductPrice> ProductPrices { get; set; }

        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }
    }
}
