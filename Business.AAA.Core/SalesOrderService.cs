using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Extensions;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using dbentities = DataAccess.Database.InventoryAAA;
using DataAccess.Repository.InventoryAAA.Interface;

namespace Business.AAA.Core
{
    public partial class SalesOrderService
    {
        IProductServices _productServices;
        IOrderServices _orderServices;

        public SalesOrderService(
        IProductServices productServices,
        IOrderServices orderServices)
        {
            this._productServices = productServices;
            this._orderServices = orderServices;
        }
    }

    public partial class SalesOrderService : IOrderTransactionalServices
    {
        public long UpdateOrderTransaction(OrderTransactionRequest orderTransactionRequest,
            List<ProductDetailRequest> orderTransactionDetailRequest)
        {
            decimal totalAmount = 0.00m, totalQuantity = 0.00m;
            long salesOrderId = 0, successReturn = 0;


            foreach (var orderDetail in orderTransactionDetailRequest)
            {
                totalAmount += totalAmount + orderDetail.UnitPrice;
                totalQuantity += totalQuantity + orderDetail.Quantity;
            }

            orderTransactionRequest.TotalQuantity = totalQuantity;
            orderTransactionRequest.TotalAmount = totalAmount;

            #region Sales Order
            var salesOrderRequest = new SalesOrdersRequest()
            {
                SalesOrderId = 0,
                SalesOrderTypeId = LookupKey.OrderType.Single,
                TotalAmount = orderTransactionRequest.TotalAmount,
                TotalQuantity = orderTransactionRequest.TotalQuantity,
                CreatedBy = orderTransactionRequest.CreatedBy,
                CreatedTime = DateTime.Now
            };

            salesOrderId = _orderServices.SaveSalesOrder(salesOrderRequest);

            if (salesOrderId <= 0)
            {
                return successReturn = 1;
            }
            #endregion

            foreach (var orderDetail in orderTransactionDetailRequest)
            {
                var productDetailResult = _productServices.GetAll().Where(p => p.ProductId == orderDetail.ProductId).FirstOrDefault();

                #region Product
                var productDetailRequest = new ProductDetailRequest()
                {
                    ProductId = productDetailResult.ProductId,
                    ProductCode = productDetailResult.ProductCode,
                    ProductDescription = productDetailResult.ProductDescription,
                    Quantity = (productDetailResult.Quantity - orderDetail.Quantity),
                    UnitPrice = productDetailResult.UnitPrice,
                    IsActive = productDetailResult.IsActive,
                    CreatedBy = productDetailResult.CreatedBy,
                    CreatedTime = productDetailResult.CreatedTime,
                    ModifiedBy = orderDetail.CreatedBy,
                    ModifiedTime = DateTime.Now
                };
                var isProductUpdated = _productServices.UpdateDetails(productDetailRequest);

                if (!isProductUpdated)
                {
                    return successReturn = 2;
                }
                #endregion

                #region Product Log
                var productLogDetailRequest = new ProductLogDetailRequest()
                {
                    ProductLogsId = 0,
                    ProductId = orderDetail.ProductId,
                    ProductCode = productDetailRequest.ProductCode,
                    ProductDescription = productDetailRequest.ProductDescription,
                    Quantity = productDetailRequest.Quantity,
                    UnitPrice = productDetailRequest.UnitPrice,
                    IsActive = productDetailRequest.IsActive,
                    CreatedBy = orderDetail.CreatedBy,
                    CreatedTime = DateTime.Now
                };

                var productLogsId = _productServices.SaveProductLogs(productLogDetailRequest);

                if (productLogsId <= 0)
                {
                    return successReturn = 3;
                }
                #endregion

                #region Sales Order Details
                SalesOrderDetailsRequest salesOrderDetailsRequest = new SalesOrderDetailsRequest()
                {
                    SalesOrderId = salesOrderId,
                    ProductId = productDetailResult.ProductId,
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                    CreatedBy = orderDetail.CreatedBy,
                    CreatedTime = DateTime.Now,
                    ModifiedBy = null,
                    ModifiedTime = null
                };

                var salesOrderDetailId = _orderServices.SaveSalesOrderDetails(salesOrderDetailsRequest);

                if (salesOrderDetailId <= 0)
                {
                    return successReturn = 4;
                }
                #endregion
            }
            return successReturn;
        }
    }
}
