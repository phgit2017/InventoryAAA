using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Extensions;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using dbentities = DataAccess.Database.InventoryAAA;
using DataAccess.Database.InventoryAAA;
using DataAccess.Repository.InventoryAAA.Interface;
using Infrastructure.Utilities;
using System.Data.SqlClient;
using System.Data;

namespace Business.AAA.Core
{
    public partial class ProductServices
    {
        IInventoryAAARepository<dbentities.Product> _productServices;
        IInventoryAAARepository<dbentities.ProductLog> _productLogServices;

        private dbentities.Product products;
        private dbentities.ProductLog productLogs;

        public ProductServices(IInventoryAAARepository<dbentities.Product> productServices,
            IInventoryAAARepository<dbentities.ProductLog> productLogServices)
        {
            this._productServices = productServices;
            this._productLogServices = productLogServices;

            this.products = new dbentities.Product();
            this.productLogs = new dbentities.ProductLog();
        }
    }

    public partial class ProductServices : IProductServices
    {
        public IQueryable<ProductDetail> GetAll()
        {
            var result = from det in this._productServices.GetAll()
                         select new ProductDetail()
                         {
                             ProductId = det.ProductID,
                             ProductCode = det.ProductCode,
                             ProductDescription = det.ProductDescription,
                             Quantity = det.Quantity,
                             UnitPrice = det.UnitPrice,
                             IsActive = det.IsActive,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime
                         };

            return result;
        }

        public List<StocksSummary> RetrieveInventorySummary()
        {
            
            var result = CommonExtensions.ConvertDataTable<StocksSummary>(
                (this._productServices.ExecuteSPReturnTable("INV_InventorySummary",
                true,
                new SqlParameter[] { })));


            return result;
        }

        public List<StocksDetails> RetrieveInventoryDetails(StocksDetailsRequest request)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@ProductID", SqlDbType.BigInt) { Value = request.ProductId }
            };

            var result = CommonExtensions.ConvertDataTable<StocksDetails>(
                (this._productServices.ExecuteSPReturnTable("INV_InventoryListDetails",
                true,
                parameters)));


            return result;
        }

        public long SaveDetails(ProductDetailRequest request)
        {
            request.ProductId = 0;
            this.products = request.DtoToEntity();
            var item = this._productServices.Insert(this.products);
            if (item == null)
            {
                return 0;
            }

            return item.ProductID;
        }

        public long SaveProductLogs(ProductLogDetailRequest request)
        {
            request.ProductLogsId = 0;
            this.productLogs = request.DtoToEntity();
            var item = this._productLogServices.Insert(this.productLogs);
            if (item == null)
            {
                return 0;
            }

            return item.ProductLogsID;
        }

        public bool UpdateDetails(ProductDetailRequest request)
        {
            this.products = request.DtoToEntity();
            var item = _productServices.Update2(this.products);
            if (item == null)
            {
                return false;
            }

            return true;
        }
    }
}
