using Business.AAA.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Interface
{
    public interface IOrderServices
    {
        IQueryable<PurchaseOrders> GetAllPurchaseOrders();
        IQueryable<SalesOrders> GetAllSalesOrders();

        IQueryable<PurchaseOrderDetails> GetAllPurchaseOrderDetails();
        IQueryable<SalesOrderDetails> GetAllSalesOrderDetails();
        
        long SavePurchaseOrder(PurchaseOrdersRequest request);
        long SavePurchaseOrderDetails(PurchaseOrderDetailsRequest request);

        long SaveSalesOrder(SalesOrdersRequest request);
        bool UpdateSalesOrder(SalesOrdersRequest request);
        long SaveSalesOrderDetails(SalesOrderDetailsRequest request);
        bool DeleteSalesOrderDetails(long salesOrderId);

        long SaveCorrectionOrder(CorrectionOrdersRequest request);
        long SaveCorrectionOrderDetails(CorrectionOrderDetailsRequest request);
        object SalesDetails(long salesOrderId);


    }
}
