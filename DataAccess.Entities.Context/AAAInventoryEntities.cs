using DataAccess.Database.InventoryAAA;
using DataAccess.Entities.Context.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Context
{
    public class AAAInventoryEntities : System.Data.Entity.DbContext, IAAAInventoryEntities
    {
        public AAAInventoryEntities()
            : base("name=AAAInventoryEntities")
        { }

        public virtual DbSet<OrderType> OrderTypes { get; set; }
        public virtual DbSet<ProductLog> ProductLogs { get; set; }
        public virtual DbSet<PriceType> PriceTypes { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<ProductPrice> ProductPrices { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public virtual DbSet<SalesOrder> SalesOrders { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }
        public virtual DbSet<UserRoleDetail> UserRoleDetails { get; set; }
        public virtual DbSet<UserInformationDetail> UserInformationDetails { get; set; }
        public virtual DbSet<MenuDetail> MenuDetails { get; set; }
        public virtual DbSet<UserMenuRoleDetail> UserMenuRoleDetails { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerStatus> CustomerStatus { get; set; }
        public virtual DbSet<CorrectionOrder> CorrectionOrders { get; set; }
        public virtual DbSet<CorrectionOrderDetail> CorrectionOrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PriceType>()
                .HasMany(e => e.ProductPrices)
                .WithRequired(e => e.PriceTypes)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Product>()
                .HasMany(e => e.ProductPrices)
                .WithRequired(e => e.Products)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

    }

    
}
