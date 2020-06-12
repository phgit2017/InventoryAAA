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
    public partial class PurchaseOrderService
    {
        IProductServices _productServices;
        IOrderServices _orderServices;

        public PurchaseOrderService(
        IProductServices productServices,
        IOrderServices orderServices)
        {
            this._productServices = productServices;
            this._orderServices = orderServices;
        }
    }

    public partial class PurchaseOrderService : IOrderTransactionalServices
    {
        public long UpdateOrderTransaction(OrderTransactionRequest orderTransactionRequest,
            List<ProductDetailRequest> orderTransactionDetailRequest
            )
        {

            decimal totalAmount = 0.00m, totalQuantity = 0.00m;
            long purchaseOrderId = 0, successReturn = 0;
            ProductDetail productDetailResult = new ProductDetail();

            foreach (var orderDetail in orderTransactionDetailRequest)
            {
                totalAmount += totalAmount + orderDetail.UnitPrice;
                totalQuantity += totalQuantity + orderDetail.Quantity;
            }

            orderTransactionRequest.TotalQuantity = totalQuantity;
            orderTransactionRequest.TotalAmount = (totalAmount * totalQuantity);

            #region Validate if Product Code is existing

            foreach (var orderDetail in orderTransactionDetailRequest)
            {
                if (orderDetail.ProductId == 0)
                {
                    var codeProductDetailResult = _productServices.GetAll().Where(p => p.ProductCode == orderDetail.ProductCode
                                                                                                    && p.IsActive).FirstOrDefault();

                    #region Validate same product code
                    if (!codeProductDetailResult.IsNull())
                    {
                        return successReturn = -100;
                    }
                    #endregion
                }
                else
                {
                    var codeProductDetailResult = _productServices.GetAll().Where(p => p.ProductCode == orderDetail.ProductCode
                                                                                && p.IsActive
                                                                                && p.ProductId != orderDetail.ProductId).FirstOrDefault();

                    #region Validate same product code
                    if (!codeProductDetailResult.IsNull())
                    {
                        return successReturn = -100;
                    }
                    #endregion
                }

            }

            #endregion

            #region Purchase Order
            var purchaseOrderRequest = new PurchaseOrdersRequest()
            {
                PurchaseOrderId = 0,
                PurchaseOrderTypeId = LookupKey.OrderType.Single,
                TotalAmount = orderTransactionRequest.TotalAmount,
                TotalQuantity = orderTransactionRequest.TotalQuantity,
                CreatedBy = orderTransactionRequest.CreatedBy,
                CreatedTime = DateTime.Now
            };

            purchaseOrderId = _orderServices.SavePurchaseOrder(purchaseOrderRequest);

            if (purchaseOrderId <= 0)
            {
                return successReturn = 1;
            }
            #endregion


            foreach (var orderDetail in orderTransactionDetailRequest)
            {
                long productId = 0;

                #region Product
                var productDetailRequest = new ProductDetailRequest()
                {
                    ProductId = orderDetail.ProductId,
                    ProductCode = orderDetail.ProductCode,
                    ProductDescription = orderDetail.ProductDescription,
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                    IsActive = orderDetail.IsActive,
                    CreatedBy = orderDetail.CreatedBy,
                    CreatedTime = DateTime.Now,
                    ModifiedBy = null,
                    ModifiedTime = null
                };

                if (orderDetail.ProductId == 0)
                {
                    productId = _productServices.SaveDetails(productDetailRequest);
                }
                else
                {
                    productDetailResult = _productServices.GetAll().Where(p => p.ProductId == orderDetail.ProductId).FirstOrDefault();

                    productDetailRequest = new ProductDetailRequest()
                    {
                        ProductId = productDetailResult.ProductId,
                        ProductCode = productDetailResult.ProductCode,
                        ProductDescription = productDetailResult.ProductDescription,
                        Quantity = (productDetailResult.Quantity + orderDetail.Quantity),
                        UnitPrice = productDetailResult.UnitPrice,
                        IsActive = productDetailResult.IsActive,
                        CreatedBy = productDetailResult.CreatedBy,
                        CreatedTime = productDetailResult.CreatedTime,
                        ModifiedBy = orderDetail.CreatedBy,
                        ModifiedTime = DateTime.Now
                    };



                    _productServices.UpdateDetails(productDetailRequest);
                    productId = productDetailResult.ProductId;
                }


                if (productId <= 0)
                {
                    return successReturn = 2;
                }
                #endregion

                #region Product Log
                var productLogDetailRequest = new ProductLogDetailRequest()
                {
                    ProductLogsId = 0,
                    ProductId = productId,
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

                #region Purchase Order Details
                PurchaseOrderDetailsRequest purchaseOrderDetailRequest = new PurchaseOrderDetailsRequest()
                {
                    PurchaseOrderId = purchaseOrderId,
                    ProductId = productId,
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                    CreatedBy = orderDetail.CreatedBy,
                    CreatedTime = DateTime.Now,
                    ModifiedBy = null,
                    ModifiedTime = null,
                    PreviousQuantity = (orderDetail.ProductId == 0) ? 0 : productDetailResult.Quantity,
                    Remarks = orderDetail.Remarks
                };

                var purchaseOrderDetailId = _orderServices.SavePurchaseOrderDetails(purchaseOrderDetailRequest);

                if (purchaseOrderDetailId <= 0)
                {
                    return successReturn = 4;
                }
                #endregion
            }



            return successReturn;
        }
    }
}
