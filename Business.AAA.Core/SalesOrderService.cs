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
using Infrastructure.Utilities;

namespace Business.AAA.Core
{
    public partial class SalesOrderService
    {
        IProductServices _productServices;
        IOrderServices _orderServices;
        ICustomerServices _customerServices;

        public SalesOrderService(
        IProductServices productServices,
        IOrderServices orderServices,
        ICustomerServices customerServices)
        {
            this._productServices = productServices;
            this._orderServices = orderServices;
            this._customerServices = customerServices;
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
            orderTransactionRequest.TotalAmount = (totalAmount * totalQuantity);

            #region Sales Order
            var salesOrderRequest = new SalesOrdersRequest()
            {
                SalesOrderId = orderTransactionRequest.SalesOrderId,
                SalesOrderTypeId = LookupKey.OrderType.Single,
                SalesNo = orderTransactionRequest.SalesOrderId== 0? GenerateSalesNumber() :  orderTransactionRequest.SalesNo,
                SalesOrderStatusId = orderTransactionRequest.SalesOrderStatusId,
                CustomerId = orderTransactionRequest.CustomerId,
                TotalAmount = orderTransactionRequest.TotalAmount,
                TotalQuantity = orderTransactionRequest.TotalQuantity,
                CreatedBy = orderTransactionRequest.CreatedBy,
                CreatedTime = DateTime.Now,
                ModeOfPayment = orderTransactionRequest.ModeOfPayment,
                ShippingFee = orderTransactionRequest.ShippingFee
            };

            if (orderTransactionRequest.SalesOrderId == 0)
                salesOrderId = _orderServices.SaveSalesOrder(salesOrderRequest);
            else
            {
                if(_orderServices.UpdateSalesOrder(salesOrderRequest))
                    salesOrderId = salesOrderRequest.SalesOrderId;
                else
                    salesOrderId = 0;
            }
                

            if (salesOrderId <= 0)
            {
                return successReturn = 1;
            }
            #endregion

            if (orderTransactionRequest.SalesOrderId == 0)
            {
                foreach (var orderDetail in orderTransactionDetailRequest)
                {
                    var productDetailResult = _productServices.GetAll().Where(p => p.ProductId == orderDetail.ProductId).FirstOrDefault();

                    if (orderTransactionRequest.SalesOrderStatusId == LookupKey.SalesOrderStatus.DeliveredId)
                    {
                        #region Product
                        var productDetailRequest = new ProductDetailRequest()
                        {
                            ProductId = productDetailResult.ProductId,
                            ProductCode = productDetailResult.ProductCode,
                            ProductDescription = productDetailResult.ProductDescription,
                            Quantity = (productDetailResult.Quantity - orderDetail.Quantity),
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
                            //UnitPrice = productDetailRequest.UnitPrice,
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
                    }

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
                        ModifiedTime = null,
                        PreviousQuantity = productDetailResult.Quantity,
                        Remarks = orderDetail.Remarks
                    };

                    var salesOrderDetailId = _orderServices.SaveSalesOrderDetails(salesOrderDetailsRequest);

                    if (salesOrderDetailId <= 0)
                    {
                        return successReturn = 4;
                    }
                    #endregion
                }
            }
            else
            {
                _orderServices.DeleteSalesOrderDetails(salesOrderId);

                foreach (var orderDetail in orderTransactionDetailRequest)
                {
                    var productDetailResult = _productServices.GetAll().Where(p => p.ProductId == orderDetail.ProductId).FirstOrDefault();

                    if (orderTransactionRequest.SalesOrderStatusId == LookupKey.SalesOrderStatus.DeliveredId)
                    {
                        #region Product
                        var productDetailRequest = new ProductDetailRequest()
                        {
                            ProductId = productDetailResult.ProductId,
                            ProductCode = productDetailResult.ProductCode,
                            ProductDescription = productDetailResult.ProductDescription,
                            Quantity = (productDetailResult.Quantity - orderDetail.Quantity),
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
                            //UnitPrice = productDetailRequest.UnitPrice,
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
                    }

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
                        ModifiedTime = null,
                        PreviousQuantity = productDetailResult.Quantity,
                        Remarks = orderDetail.Remarks,
                        PriceTypeId = orderDetail.PriceTypeId
                    };

                    
                    var salesOrderDetailId = _orderServices.SaveSalesOrderDetails(salesOrderDetailsRequest);
                    if (salesOrderDetailId <= 0)
                    {
                        return successReturn = 4;
                    }
                    #endregion
                }
            }

            orderTransactionRequest.SalesOrderId = salesOrderId;
            return successReturn;
        }

        private string GenerateSalesNumber()
        {
            string formattedDateToday = DateTime.Now.ToString("yyyyMMdd");
            string salesNumberValue = "";
            DateTime dateTimeToday = DateTime.Now;
            long maxCount = 1;

            var maxSalesOrderOfTheDay = _orderServices.GetAllSalesOrders().OrderByDescending(m => m.SalesOrderId).FirstOrDefault();
            if (!maxSalesOrderOfTheDay.IsNull())
            {
                var format = "";
                DateTime dt1 = new DateTime(dateTimeToday.Year, dateTimeToday.Month, dateTimeToday.Day);
                DateTime dt2 = new DateTime(maxSalesOrderOfTheDay.CreatedTime.Value.Year, maxSalesOrderOfTheDay.CreatedTime.Value.Month, maxSalesOrderOfTheDay.CreatedTime.Value.Day);

                var comparedValue = DateTime.Compare(dt1, dt2);
                if (comparedValue == 0)
                {
                    format = string.IsNullOrEmpty(maxSalesOrderOfTheDay.SalesNo) ? "00000001" : maxSalesOrderOfTheDay.SalesNo;
                    maxCount = Convert.ToInt64(format.Substring(8));
                    maxCount = maxCount + 1;
                }
                else
                {
                    format = string.IsNullOrEmpty(maxSalesOrderOfTheDay.SalesNo) ? "00000001" : "00000001";
                    maxCount = Convert.ToInt64(format);
                }

                
            }
            
            salesNumberValue = formattedDateToday + maxCount.ToString("D6");
            return salesNumberValue;
        }
    }
}
