namespace DataAccess.Database.InventoryAAA
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        public Product()
        {
            ProductLogs = new HashSet<ProductLog>();
            PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
            SalesOrderDetails = new HashSet<SalesOrderDetail>();
            ProductPrices = new HashSet<ProductPrice>();
        }

        public long ProductID { get; set; }

        [StringLength(32)]
        public string ProductCode { get; set; }

        [StringLength(32)]
        public string ProductDescription { get; set; }

        public decimal Quantity { get; set; }

        public bool IsActive { get; set; }

        public long? CategoryID { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedTime { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Categories Category { get; set; }

        public virtual ICollection<ProductLog> ProductLogs { get; set; }

        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; }

        //[InverseProperty("Product_ProductID")]
        public virtual ICollection<ProductPrice> ProductPrices { get; set; }
    }
}
