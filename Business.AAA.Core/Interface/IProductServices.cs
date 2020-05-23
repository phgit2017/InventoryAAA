using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Dto;

namespace Business.AAA.Core.Interface
{
    public interface IProductServices
    {
        IQueryable<ProductDetail> GetAll();
        long SaveDetails(ProductDetailRequest request);
        bool UpdateDetails(ProductDetailRequest request);
        long SaveProductLogs(ProductLogDetailRequest request);
        List<StocksSummary> RetrieveInventorySummary();
        List<StocksDetails> RetrieveInventoryDetails(StocksDetailsSearchRequest request);

        //Reporting
        DataSet SalesReport(DateTime startDate, DateTime endDate);
        DataTable PurchaseandSalesReport(DateTime startDate, DateTime endDate);
    }
}
