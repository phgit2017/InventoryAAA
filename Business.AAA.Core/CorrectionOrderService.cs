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
    public partial class CorrectionOrderService
    {
        IProductServices _productServices;
        IOrderServices _orderServices;
        ICustomerServices _customerServices;

        public CorrectionOrderService(
        IProductServices productServices,
        IOrderServices orderServices,
        ICustomerServices customerServices)
        {
            this._productServices = productServices;
            this._orderServices = orderServices;
            this._customerServices = customerServices;
        }

    }

    public partial class CorrectionOrderService : IOrderTransactionalServices
    {
        public long UpdateOrderTransaction(OrderTransactionRequest orderTransactionRequest,
            List<ProductDetailRequest> orderTransactionDetailRequest)
        {
            decimal totalAmount = 0.00m, totalQuantity = 0.00m;
            long correctionOrderId = 0, successReturn = 0;
            ProductDetail productDetailResult = new ProductDetail();

            foreach (var orderDetail in orderTransactionDetailRequest)
            {
                //totalAmount += totalAmount + orderDetail.UnitPrice;
                totalQuantity += totalQuantity + orderDetail.Quantity;
            }

            orderTransactionRequest.TotalQuantity = totalQuantity;
            orderTransactionRequest.TotalAmount = (totalAmount * totalQuantity);

            #region Validate if Product Code is existing

            foreach (var orderDetail in orderTransactionDetailRequest)
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

            #endregion

            #region Correction Order
            var correctionOrdersRequest = new CorrectionOrdersRequest()
            {
                CorrectionOrderId = 0,
                CorrectionOrderTypeId = LookupKey.OrderType.Single,
                TotalAmount = orderTransactionRequest.TotalAmount,
                TotalQuantity = orderTransactionRequest.TotalQuantity,
                CreatedBy = orderTransactionRequest.CreatedBy,
                CreatedTime = DateTime.Now
            };

            correctionOrderId = _orderServices.SaveCorrectionOrder(correctionOrdersRequest);

            if (correctionOrderId <= 0)
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
                    //UnitPrice = orderDetail.UnitPrice,
                    IsActive = orderDetail.IsActive,
                    CreatedBy = orderDetail.CreatedBy,
                    CategoryId = orderDetail.CategoryId,
                    CreatedTime = DateTime.Now,
                    ModifiedBy = null,
                    ModifiedTime = null
                };

                productDetailResult = _productServices.GetAll().Where(p => p.ProductId == orderDetail.ProductId).FirstOrDefault();

                productDetailRequest = new ProductDetailRequest()
                {
                    ProductId = productDetailResult.ProductId,
                    ProductCode = productDetailResult.ProductCode,
                    ProductDescription = productDetailResult.ProductDescription,
                    Quantity =  orderDetail.Quantity,
                    //UnitPrice = productDetailResult.UnitPrice,
                    IsActive = productDetailResult.IsActive,
                    CategoryId = productDetailResult.CategoryId,
                    CreatedBy = productDetailResult.CreatedBy,
                    CreatedTime = productDetailResult.CreatedTime,
                    ModifiedBy = orderDetail.CreatedBy,
                    ModifiedTime = DateTime.Now
                };



                _productServices.UpdateDetails(productDetailRequest);
                productId = productDetailResult.ProductId;

                #region Product Price
                for (int i = 1; i <= 3; i++)
                {
                    var price = 0.0m;
                    switch (i)
                    {
                        case 1:
                            price = orderDetail.BigBuyerPrice;
                            break;
                        case 2:
                            price = orderDetail.ResellerPrice;
                            break;
                        case 3:
                            price = orderDetail.RetailerPrice;
                            break;
                        default:
                            price = 0;
                            break;
                    }
                    ProductPricesDetailRequest productPricesDetailRequest = new ProductPricesDetailRequest()
                    {
                        ProductId = productId,
                        PriceTypeId = i,
                        Price = price
                    };
                    _productServices.UpdateProductPrice(productPricesDetailRequest);
                }
                #endregion



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
                    CategoryId = productDetailRequest.CategoryId,
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

                #region Correction Order Details
                CorrectionOrderDetailsRequest correctionOrderDetailsRequest = new CorrectionOrderDetailsRequest()
                {
                    CorrectionOrderId = correctionOrderId,
                    ProductId = productId,
                    Quantity = orderDetail.Quantity,
                    UnitPrice = 0,
                    CreatedBy = orderDetail.CreatedBy,
                    CreatedTime = DateTime.Now,
                    ModifiedBy = null,
                    ModifiedTime = null,
                    PreviousQuantity = productDetailResult.Quantity,
                    Remarks = orderDetail.Remarks
                };

                var correctionOrderDetailId = _orderServices.SaveCorrectionOrderDetails(correctionOrderDetailsRequest);

                if (correctionOrderDetailId <= 0)
                {
                    return successReturn = 4;
                }
                #endregion
            }

            
            return successReturn;
        }
    }
}
