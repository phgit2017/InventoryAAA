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
        IInventoryAAARepository<dbentities.ProductPrice> _productPriceServices;
        IInventoryAAARepository<dbentities.ProductPricesLog> _productPricesLogServices;

        private dbentities.Product products;
        private dbentities.ProductLog productLogs;
        private dbentities.ProductPrice productPrice;
        private dbentities.ProductPricesLog productPricesLog;

        public ProductServices(
            IInventoryAAARepository<dbentities.Product> productServices,
            IInventoryAAARepository<dbentities.ProductLog> productLogServices,
            IInventoryAAARepository<dbentities.ProductPrice> productPriceServices,
            IInventoryAAARepository<dbentities.ProductPricesLog> productPricesLogServices)
        {
            this._productServices = productServices;
            this._productLogServices = productLogServices;
            this._productPriceServices = productPriceServices;
            this._productPricesLogServices = productPricesLogServices;

            this.products = new dbentities.Product();
            this.productLogs = new dbentities.ProductLog();
            this.productPrice = new dbentities.ProductPrice();
            this.productPricesLog = new dbentities.ProductPricesLog();
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
                             IsActive = det.IsActive,
                             CategoryId = det.CategoryID,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime
                         };

            return result;
        }

        public IQueryable<ProductPricesDetail> GetAllProductPrices()
        {
            var result = from det in this._productPriceServices.GetAll()
                         select new ProductPricesDetail()
                         {
                             ProductId = det.ProductID,
                             ProductCode = det.Products.ProductCode,
                             ProductDescription = det.Products.ProductDescription,
                             Price = det.Price,
                             PriceTypeId = det.PriceTypeID,
                             PriceTypeName = det.PriceTypes.PriceTypeName
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

        public List<StocksSummaryForSalesOrders> RetrieveInventorySummaryForSalesOrders()
        {
            var result = CommonExtensions.ConvertDataTable<StocksSummaryForSalesOrders>(
                (this._productServices.ExecuteSPReturnTable("INV_InventorySummaryForSalesOrder",
                true,
                new SqlParameter[] { })));


            return result;
            
        }

        public List<StocksDetails> RetrieveInventoryDetails(StocksDetailsSearchRequest request)
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

        public long SaveProductPrice(ProductPricesDetailRequest request)
        {
            //request.ProductId = 0;
            this.productPrice = request.DtoToEntity();
            var item = this._productPriceServices.Insert(this.productPrice);
            if (item == null)
            {
                return 0;
            }

            return item.ProductID;
        }

        public bool UpdateProductPrice(ProductPricesDetailRequest request)
        {
            this.productPrice = request.DtoToEntity();
            var item = this._productPriceServices.Update2(this.productPrice);
            if (item == null)
            {
                return false;
            }

            return true;
        }

        public long SaveProductLogPrices(ProductPricesLogDetailRequest request)
        {
            //request.ProductId = 0;
            this.productPricesLog = request.DtoToEntity();
            var item = this._productPricesLogServices.Insert(this.productPricesLog);
            if (item == null)
            {
                return 0;
            }

            return item.ProductPriceLogsID;
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

    /// <summary>
    /// Reports go over here
    /// </summary>
    public partial class ProductServices : IProductServices
    {
        public DataSet SalesReport(DateTime startDate, DateTime endDate)
        {
            DataSet ds = new DataSet();
            SqlParameter[] sqlParams = new SqlParameter[]
            {
                new SqlParameter() { ParameterName = "StartDate", Value = startDate, SqlDbType=  SqlDbType.DateTime },
                new SqlParameter() { ParameterName = "EndDate", Value = endDate, SqlDbType = SqlDbType.DateTime },
            };

            ds = this._productServices.ExecuteSPReturnSet("INV_SalesReport", true, sqlParams);

            return ds;
        }

        public DataTable PurchaseandSalesReport(DateTime startDate, DateTime endDate)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParams = new SqlParameter[]
            {
                new SqlParameter() { ParameterName = "StartDate", Value = startDate, SqlDbType=  SqlDbType.DateTime },
                new SqlParameter() { ParameterName = "EndDate", Value = endDate, SqlDbType = SqlDbType.DateTime },
            };

            dt = this._productServices.ExecuteSPReturnTable("INV_PurchaseAndSalesReport", true, sqlParams);

            return dt;
        }

        public DataTable SalesReportPerSalesNo(string salesNo,long salesOrderId = 0)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParams = new SqlParameter[]
            {
                new SqlParameter() { ParameterName = "SalesNo", Value = salesNo, SqlDbType=  SqlDbType.NVarChar },
                new SqlParameter() { ParameterName = "SalesOrderId", Value = salesOrderId, SqlDbType=  SqlDbType.BigInt },
            };

            dt = this._productServices.ExecuteSPReturnTable("INV_SalesOrderReport", true, sqlParams);

            return dt;
        }

        public DataTable SalesReportPerCustomerId(DateTime? startDate, DateTime? endDate, long customerId = 0,int salesOrderStatusId = 0)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParams = new SqlParameter[]
            {
                new SqlParameter() { ParameterName = "StartDate", Value = startDate, SqlDbType=  SqlDbType.DateTime },
                new SqlParameter() { ParameterName = "EndDate", Value = endDate, SqlDbType = SqlDbType.DateTime },
                new SqlParameter() { ParameterName = "CustomerId", Value = customerId, SqlDbType=  SqlDbType.BigInt },
                new SqlParameter() { ParameterName = "SalesOrderStatusId", Value = salesOrderStatusId, SqlDbType=  SqlDbType.Int }
            };

            dt = this._productServices.ExecuteSPReturnTable("INV_SalesOrderReportPerCustomer", true, sqlParams);

            return dt;
        }

        public DataTable SalesReportPerCategoryAndDate(DateTime? startDate, DateTime? endDate, long categoryId = 0, int salesOrderStatusId = 0)
        {
            DataTable dt = new DataTable();
            SqlParameter[] sqlParams = new SqlParameter[]
            {
                new SqlParameter() { ParameterName = "StartDate", Value = startDate, SqlDbType=  SqlDbType.DateTime },
                new SqlParameter() { ParameterName = "EndDate", Value = endDate, SqlDbType = SqlDbType.DateTime },
                new SqlParameter() { ParameterName = "CategoryId", Value = categoryId, SqlDbType = SqlDbType.BigInt },
                new SqlParameter() { ParameterName = "SalesOrderStatusId", Value = salesOrderStatusId, SqlDbType=  SqlDbType.Int }
            };

            dt = this._productServices.ExecuteSPReturnTable("INV_SalesOrderReportPerCategoryAndDate", true, sqlParams);

            return dt;
        }
    }
}
