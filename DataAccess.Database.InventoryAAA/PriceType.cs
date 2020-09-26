namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PriceType
    {
        public PriceType()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PriceTypeID { get; set; }

        [Required]
        [StringLength(32)]
        public string PriceTypeName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
