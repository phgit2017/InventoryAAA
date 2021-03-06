﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

using Business.AAA.Core;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using Infrastructure.Utilities;
using Inventory_AAA.Infrastructure;
using Inventory_AAA.Models;
using LinqKit;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Inventory_AAA.Controllers
{
    public class StocksController : Controller
    {
        private readonly IOrderTypeServices _orderTypeServices;
        private readonly IOrderTransactionalServices _orderTransactionalServices;
        private readonly IProductServices _productServices;
        private readonly IOrderServices _orderServices;
        private readonly IUserServices _userServices;
        private readonly ICustomerServices _customerServices;

        public StocksController(
            IOrderTypeServices orderTypeServices,
            IOrderTransactionalServices orderTransactionalServices,
            IProductServices productServices,
            IOrderServices orderServices,
            IUserServices userServices,
            ICustomerServices customerServices)
        {
            this._orderTypeServices = orderTypeServices;
            this._orderTransactionalServices = orderTransactionalServices;
            this._productServices = productServices;
            this._orderServices = orderServices;
            this._userServices = userServices;
            this._customerServices = customerServices;
        }

        // GET: Stocks
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult InventorySummary()
        {
            List<StocksSummary> result = new List<StocksSummary>();

            #region Authorize
            var authorizeMenuAccessResult = AuthorizeMenuAccess(LookupKey.Menu.InventoryMenuId);
            if (!authorizeMenuAccessResult.IsSuccess)
            {

                return Json(new
                {
                    isSuccess = authorizeMenuAccessResult.IsSuccess,
                    messageAlert = authorizeMenuAccessResult.MessageAlert,
                    result = result
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            result = this._productServices.RetrieveInventorySummary();

            var response = new
            {
                isSuccess = true,
                messageAlert = string.Empty,
                result = result,
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InventorySummaryForSalesOrders()
        {
            List<StocksSummaryForSalesOrders> result = new List<StocksSummaryForSalesOrders>();

            #region Authorize
            var authorizeMenuAccessResult = AuthorizeMenuAccess(LookupKey.Menu.InventoryMenuId);
            if (!authorizeMenuAccessResult.IsSuccess)
            {

                return Json(new
                {
                    isSuccess = authorizeMenuAccessResult.IsSuccess,
                    messageAlert = authorizeMenuAccessResult.MessageAlert,
                    result = result
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            result = this._productServices.RetrieveInventorySummaryForSalesOrders();

            var response = new
            {
                isSuccess = true,
                messageAlert = string.Empty,
                result = result,
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InventoryDetails(StocksDetailsSearchRequest request)
        {

            //Product Details
            var productResult = _productServices.GetAll().Where(p => p.ProductId == request.ProductId).FirstOrDefault();
            productResult.ProductPrices = _productServices.GetAllProductPrices().Where(m => m.ProductId == request.ProductId).ToList();

            //Purchase order and Sales order Details
            //var inventoryDetailsResult = _productServices.RetrieveInventoryDetails(request);

            var response = new
            {
                ProductResult = productResult,
                //InventoryDetailsResult = inventoryDetailsResult,
                isSuccess = true
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ProductDetails(int productId)
        {
            //Product Details
            var productResult = _productServices.GetAll().Where(p => p.ProductId == productId).FirstOrDefault();

            var response = new
            {
                ProductResult = productResult,
                isSuccess = true
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateInventoryOrder(OrderRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty, orderTransactionTypeService = string.Empty;
            long updateOrderTransactionResult = 0;

            OrderTransactionRequest orderTransactionRequest = new OrderTransactionRequest();
            List<ProductDetailRequest> orderTransactionDetailRequest = new List<ProductDetailRequest>();

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);

            if (ModelState.IsValid)
            {
                #region Set Order Transaction Type


                if (request.OrderTransactionType == LookupKey.OrderTransactionType.PurchaseOrder)
                {
                    orderTransactionTypeService = "PurchaseOrderService";
                }
                else if (request.OrderTransactionType == LookupKey.OrderTransactionType.SalesOrder)
                {
                    orderTransactionTypeService = "SalesOrderService";
                }
                else if (request.OrderTransactionType == LookupKey.OrderTransactionType.CorrectionOrder)
                {
                    orderTransactionTypeService = "CorrectionOrderService";
                }

                #endregion

                #region Service implementation

                orderTransactionRequest.CreatedBy = currentUserId;

                orderTransactionDetailRequest.Add(new ProductDetailRequest()
                {
                    ProductId = request.ProductId,
                    ProductCode = request.ProductCode,
                    ProductDescription = request.ProductDescription,
                    Quantity = request.Stocks,

                    CategoryId = request.CategoryId,
                    IsActive = request.IsActive,
                    CreatedBy = currentUserId,
                    Remarks = request.Remarks,
                    ProductPrices = request.ProductPrices,
                    RetailerPrice = request.RetailerPrice,
                    ResellerPrice = request.ResellerPrice,
                    BigBuyerPrice = request.BigBuyerPrice,

                });
                var type = Type.GetType(string.Format("{0}.{1}, {0}", "Business.AAA.Core", orderTransactionTypeService));
                IOrderTransactionalServices order = (IOrderTransactionalServices)Activator.CreateInstance(type,
                    _productServices,
                    _orderServices,
                    _customerServices);
                updateOrderTransactionResult = order.UpdateOrderTransaction(
                    ref orderTransactionRequest,
                    orderTransactionDetailRequest);

                #endregion

                //IOrderTransactionalServices x = new PurchaseOrderService(_productServices, _orderServices);
                //var type = Type.GetType("Business.AAA.Core.PurchaseOrderService, Business.AAA.Core");
                //updateOrderTransactionResult = x.UpdateOrderTransaction(orderTransactionRequest, orderTransactionDetailRequest);

                if (updateOrderTransactionResult == -100)
                {
                    return Json(new { isSucess = isSucess, messageAlert = Messages.ProductCodeValidation }, JsonRequestBehavior.AllowGet);
                }
                else if (updateOrderTransactionResult == 0)
                {
                    isSucess = true;
                }

                var response = new
                {
                    isSucess = isSucess,
                    messageAlert = messageAlert
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { isSucess = isSucess, messageAlert = Messages.ErrorOccuredDuringProcessing }, JsonRequestBehavior.AllowGet);
            }

        }

        #region Sales order
        [HttpPost]
        public JsonResult UpdateInventorySalesOrder(SalesOrderRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty, orderTransactionTypeService = string.Empty;
            long updateOrderTransactionResult = 0;
            long salesOrderIdResult = 0;

            OrderTransactionRequest orderTransactionRequest = new OrderTransactionRequest();
            List<ProductDetailRequest> orderTransactionDetailRequest = new List<ProductDetailRequest>();

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);

            if (ModelState.IsValid)
            {
                #region Set Order Transaction Type

                if (request.OrderTransactionType == LookupKey.OrderTransactionType.SalesOrder)
                {
                    orderTransactionTypeService = "SalesOrderService";
                }

                #endregion

                #region Service implementation

                orderTransactionRequest.CreatedBy = currentUserId;
                orderTransactionRequest.CustomerId = request.CustomerId;
                orderTransactionRequest.SalesOrderId = request.SalesOrderId;
                orderTransactionRequest.SalesOrderStatusId = request.SalesOrderStatusId;
                orderTransactionRequest.SalesNo = request.SalesNo;
                orderTransactionRequest.ModeOfPayment = request.ModeOfPayment;
                orderTransactionRequest.ShippingFee = request.ShippingFee;

                #region Validation for CurrentStocks
                List<long> productIdCollections = new List<long>();
                List<string> productCodeWithCorrections = new List<string>();
                foreach (var salesOrderDetails in request.SalesOrderProductDetailRequest)
                {
                    productIdCollections.Add(salesOrderDetails.ProductId);
                }

                var currentStockInventory =  _productServices.GetAll().Where(m => productIdCollections.Contains(m.ProductId)).ToList();

                foreach (var salesDetails in request.SalesOrderProductDetailRequest)
                {
                    var currentStocks = currentStockInventory.Where(m => m.ProductId == salesDetails.ProductId).FirstOrDefault();
                    if (currentStocks.Quantity < salesDetails.Quantity)
                    {
                        productCodeWithCorrections.Add(currentStocks.ProductCode);
                    }


                    
                    
                }

                if (productCodeWithCorrections.Count > 0)
                {
                    var currentStockMessage = "";
                    if (productCodeWithCorrections.Count == 1)
                    {
                        currentStockMessage = productCodeWithCorrections.FirstOrDefault().ToString();
                    }
                    else
                    {
                        for (int i = 0; i < productCodeWithCorrections.Count; i++)
                        {
                            if (i== 0)
                            {
                                currentStockMessage = productCodeWithCorrections[i].ToString();
                            }
                            else
                            {
                                currentStockMessage += ", " + productCodeWithCorrections[i].ToString();
                            }

                        }
                    }
                    var x = string.Format(Messages.CurrentStockValidation, currentStockMessage);
                    return Json(new
                    {
                        isSuccess = isSucess,
                        messageAlert = string.Format(Messages.CurrentStockValidation, currentStockMessage),
                        salesOrderIdResult = salesOrderIdResult
                    }, JsonRequestBehavior.AllowGet);
                }
                #endregion



                foreach (var salesOrderDetails in request.SalesOrderProductDetailRequest)
                {
                    orderTransactionDetailRequest.Add(new ProductDetailRequest()
                    {
                        ProductId = salesOrderDetails.ProductId,
                        UnitPrice = salesOrderDetails.UnitPrice,
                        Quantity = salesOrderDetails.Quantity,
                        PriceTypeId = salesOrderDetails.PriceTypeId,
                        CreatedBy = currentUserId,

                    });
                }
                var type = Type.GetType(string.Format("{0}.{1}, {0}", "Business.AAA.Core", orderTransactionTypeService));
                IOrderTransactionalServices order = (IOrderTransactionalServices)Activator.CreateInstance(type,
                    _productServices,
                    _orderServices,
                    _customerServices);
                updateOrderTransactionResult = order.UpdateOrderTransaction(
                    ref orderTransactionRequest,
                    orderTransactionDetailRequest);

                #endregion

                //IOrderTransactionalServices x = new PurchaseOrderService(_productServices, _orderServices);
                //var type = Type.GetType("Business.AAA.Core.PurchaseOrderService, Business.AAA.Core");
                //updateOrderTransactionResult = x.UpdateOrderTransaction(orderTransactionRequest, orderTransactionDetailRequest);

                if (updateOrderTransactionResult == -100)
                {
                    return Json(new { isSuccess = isSucess, messageAlert = Messages.ProductCodeValidation, salesOrderIdResult = salesOrderIdResult }, JsonRequestBehavior.AllowGet);
                }
                else if (updateOrderTransactionResult == 0)
                {
                    isSucess = true;

                }

                salesOrderIdResult = orderTransactionRequest.SalesOrderId;
                var response = new
                {
                    isSuccess = isSucess,
                    messageAlert = messageAlert,
                    salesOrderIdResult = salesOrderIdResult
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { isSuccess = isSucess, messageAlert = Messages.ErrorOccuredDuringProcessing, salesOrderIdResult = salesOrderIdResult }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SalesOrders()
        {
            List<SalesOrders> result = new List<SalesOrders>();
            List<int> pendingOrderStatus = new List<int>();
            pendingOrderStatus.Add(LookupKey.SalesOrderStatus.DeliveredId);
            pendingOrderStatus.Add(LookupKey.SalesOrderStatus.CancelledId);
            #region Authorize
            var authorizeMenuAccessResult = AuthorizeMenuAccess(LookupKey.Menu.SalesOrderMenuId);
            if (!authorizeMenuAccessResult.IsSuccess)
            {

                return Json(new
                {
                    isSuccess = authorizeMenuAccessResult.IsSuccess,
                    messageAlert = authorizeMenuAccessResult.MessageAlert,
                    result = result
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            result = this._orderServices.GetAllSalesOrders().Where(m => !pendingOrderStatus.Contains(m.SalesOrderStatusId)).ToList();

            var response = new
            {
                isSuccess = true,
                messageAlert = string.Empty,
                result = result,
            };
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SalesOrdersTransactionHistory(SalesOrderSearchRequest request)
        {
            List<SalesOrders> result = new List<SalesOrders>();

            #region Authorize
            var authorizeMenuAccessResult = AuthorizeMenuAccess(LookupKey.Menu.SalesOrderMenuId);
            if (!authorizeMenuAccessResult.IsSuccess)
            {

                return Json(new
                {
                    isSuccess = authorizeMenuAccessResult.IsSuccess,
                    messageAlert = authorizeMenuAccessResult.MessageAlert,
                    result = result
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            if (request.StartDate.IsNull() && request.EndDate.IsNull() && request.SalesOrderStatusId.IsNull())
            {
                request.StartDate = DateTime.Now;
                request.EndDate = DateTime.Now;
                result = this._orderServices.GetAllSalesOrders().AsExpandable().Where(a => DbFunctions.TruncateTime(a.ModifiedTime) >= DbFunctions.TruncateTime(request.StartDate) && DbFunctions.TruncateTime(a.ModifiedTime) <= DbFunctions.TruncateTime(request.EndDate)).ToList();
            }
            else
            {
                var predicate = PredicateBuilder.New<SalesOrders>();

                if (!request.StartDate.IsNull() && request.EndDate.IsNull())
                {
                    if (request.SalesOrderStatusId == LookupKey.SalesOrderStatus.PendingId)
                    {
                        predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreatedTime) >= DbFunctions.TruncateTime(request.StartDate));
                    }
                    else
                    {
                        predicate = predicate.And(a => DbFunctions.TruncateTime(a.ModifiedTime) >= DbFunctions.TruncateTime(request.StartDate));
                    }

                }
                else if (request.StartDate.IsNull() && !request.EndDate.IsNull())
                {
                    if (request.SalesOrderStatusId == LookupKey.SalesOrderStatus.PendingId)
                    {
                        predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreatedTime) <= DbFunctions.TruncateTime(request.EndDate));
                    }
                    else
                    {
                        predicate = predicate.And(a => DbFunctions.TruncateTime(a.ModifiedTime) <= DbFunctions.TruncateTime(request.EndDate));
                    }

                }
                else if (!request.StartDate.IsNull() && !request.EndDate.IsNull() && !request.SalesOrderStatusId.IsNull())
                {
                    if (request.SalesOrderStatusId == LookupKey.SalesOrderStatus.PendingId)
                    {
                        predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreatedTime) >= DbFunctions.TruncateTime(request.StartDate) && DbFunctions.TruncateTime(a.CreatedTime) <= DbFunctions.TruncateTime(request.EndDate) && a.SalesOrderStatusId == request.SalesOrderStatusId);
                    }
                    else
                    {
                        predicate = predicate.And(a => DbFunctions.TruncateTime(a.ModifiedTime) >= DbFunctions.TruncateTime(request.StartDate) && DbFunctions.TruncateTime(a.ModifiedTime) <= DbFunctions.TruncateTime(request.EndDate) && a.SalesOrderStatusId == request.SalesOrderStatusId);
                    }

                }
                else if (!request.StartDate.IsNull() && !request.EndDate.IsNull())
                {
                    if (request.SalesOrderStatusId == LookupKey.SalesOrderStatus.PendingId)
                    {
                        predicate = predicate.And(a => DbFunctions.TruncateTime(a.CreatedTime) >= DbFunctions.TruncateTime(request.StartDate) && DbFunctions.TruncateTime(a.CreatedTime) <= DbFunctions.TruncateTime(request.EndDate));
                    }
                    else
                    {
                        predicate = predicate.And(a => DbFunctions.TruncateTime(a.ModifiedTime) >= DbFunctions.TruncateTime(request.StartDate) && DbFunctions.TruncateTime(a.ModifiedTime) <= DbFunctions.TruncateTime(request.EndDate));
                    }

                }

                result = this._orderServices.GetAllSalesOrders().AsExpandable().Where(predicate).ToList();

            }


            var response = new
            {
                isSuccess = true,
                messageAlert = string.Empty,
                result = result,
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SalesOrderDetails(long salesOrderId)
        {

            //List<SalesOrderDetails> result = new List<SalesOrderDetails>();

            #region Authorize
            var authorizeMenuAccessResult = AuthorizeMenuAccess(LookupKey.Menu.SalesOrderMenuId);
            if (!authorizeMenuAccessResult.IsSuccess)
            {

                return Json(new
                {
                    isSuccess = authorizeMenuAccessResult.IsSuccess,
                    messageAlert = authorizeMenuAccessResult.MessageAlert,
                    result = new { }
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            var result = this._orderServices.SalesDetails(salesOrderId);

            var response = new
            {
                isSuccess = true,
                messageAlert = string.Empty,
                result = result,
            };


            return Json(response, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetSalesOrderReceipt(long salesOrderId)
        {

            List<SalesOrderReceiptDetail> result = new List<SalesOrderReceiptDetail>();
            result = CommonExtensions.ConvertDataTable<SalesOrderReceiptDetail>
                                                        (_productServices.SalesReportPerSalesNo(string.Empty, salesOrderId));

            var response = new
            {
                isSuccess = true,
                messageAlert = string.Empty,
                result = result,
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportSalesOrderReceipt(long salesOrderId)
        {
            var request = new SalesOrderReportRequest()
            {
                SalesOrderId = salesOrderId
            };
            var salesOrderReportGeneration = SalesOrderReportGeneration(request);
            return salesOrderReportGeneration;
        }
        #endregion

        [HttpPost]
        public JsonResult UpdateProductDetails(ProductDetailRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty;
            bool productUpdateResult = false;

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);
            var passedProductResult = _productServices.GetAll().Where(m => m.ProductId == request.ProductId).FirstOrDefault();

            request.CreatedTime = passedProductResult.CreatedTime;
            request.ModifiedBy = currentUserId;
            request.ModifiedTime = DateTime.Now;



            //if (ModelState.IsValid)
            //{
            var codeProductDetailResult = _productServices.GetAll().Where(p => p.ProductCode == request.ProductCode
                                                                            && p.IsActive
                                                                            && p.ProductId != request.ProductId).FirstOrDefault();

            #region Validate same product code
            if (!codeProductDetailResult.IsNull())
            {
                return Json(new { isSucess = isSucess, messageAlert = Messages.ProductCodeValidation }, JsonRequestBehavior.AllowGet);
            }
            #endregion


            //Update Product Details
            productUpdateResult = _productServices.UpdateDetails(request);

            if (!productUpdateResult)
            {
                return Json(new { isSucess = isSucess, messageAlert = Messages.ServerError }, JsonRequestBehavior.AllowGet);
            }

            #region Product Price
            for (int i = 1; i <= 3; i++)
            {
                var price = 0.0m;
                switch (i)
                {
                    case 1:
                        price = request.BigBuyerPrice;
                        break;
                    case 2:
                        price = request.ResellerPrice;
                        break;
                    case 3:
                        price = request.RetailerPrice;
                        break;
                    default:
                        price = 0;
                        break;
                }
                var productPricesResult = _productServices.GetAllProductPrices().Where(m => m.ProductId == request.ProductId && m.PriceTypeId == i).FirstOrDefault();
                if (!productPricesResult.IsNull())
                {
                    ProductPricesLogDetailRequest productPricesLogDetailRequest = new ProductPricesLogDetailRequest()
                    {
                        ProductId = request.ProductId,
                        PriceTypeId = i,
                        Price = price,
                        ProductPriceLogsId = 0,
                        CreatedBy = currentUserId,
                        CreatedTime = DateTime.Now,
                    };
                    _productServices.SaveProductLogPrices(productPricesLogDetailRequest);
                }
                ProductPricesDetailRequest productPricesDetailRequest = new ProductPricesDetailRequest()
                {
                    ProductId = request.ProductId,
                    PriceTypeId = i,
                    Price = price
                };
                _productServices.UpdateProductPrice(productPricesDetailRequest);
            }
            #endregion

            var productLogDetailRequest = new ProductLogDetailRequest()
            {
                ProductLogsId = 0,
                ProductId = request.ProductId,
                ProductCode = request.ProductCode,
                ProductDescription = request.ProductDescription,
                Quantity = request.Quantity,
                //UnitPrice = request.UnitPrice,
                IsActive = request.IsActive,
                CreatedBy = request.ModifiedBy,
                CreatedTime = DateTime.Now,
                ModifiedBy = null,
                ModifiedTime = null,
                CategoryId = request.CategoryId
            };
            var productLogResult = _productServices.SaveProductLogs(productLogDetailRequest);

            if (productLogResult <= 0)
            {
                return Json(new { isSucess = isSucess, messageAlert = Messages.ServerError }, JsonRequestBehavior.AllowGet);
            }

            isSucess = true;
            var response = new
            {
                isSucess = isSucess,
                messageAlert = messageAlert
            };
            return Json(response, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{

            //    var errors = ModelState.Values.SelectMany(v => v.Errors)
            //               .ToList();
            //    foreach (var err in errors)
            //    {
            //        Logging.Information("(Response-Model-Stocks) UpdateProductDetails : " + err.ErrorMessage);
            //    }

            //    return Json(new { isSucess = isSucess, messageAlert = Messages.ErrorOccuredDuringProcessing }, JsonRequestBehavior.AllowGet);
            //}



        }

        public AuthorizationDetail AuthorizeMenuAccess(int menuId)
        {

            AuthorizationDetail response = new AuthorizationDetail();
            long userSessionId = 0;

            try
            {
                userSessionId = Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);

                if (userSessionId == 0)
                {
                    response.MessageAlert = Messages.SessionUnavailable;
                    return response;
                }
            }
            catch (Exception)
            {
                response.MessageAlert = Messages.SessionUnavailable;
                return response;
            }


            var userId = userSessionId;
            var userResult = _userServices.GetAllUserDetails().Where(m => m.UserId == userId).FirstOrDefault();

            #region Menu Role
            var userMenuRoleResult = _userServices.GetAllUserMenuRoleDetails().Where(m => m.MenuId == menuId
                                                        && m.RoleId == userResult.UserRoleId).FirstOrDefault();

            if (userMenuRoleResult.IsNull())
            {
                response.MessageAlert = Messages.UnauthorizeAccess;
            }
            #endregion


            if (string.IsNullOrEmpty(response.MessageAlert))
                response.IsSuccess = true;


            return response;

        }

        #region Private methods
        private FileResult SalesOrderReportGeneration(SalesOrderReportRequest request)
        {
            DataTable dt = new DataTable();

            dt = _productServices.SalesReportPerSalesNo(request.SalesNo, request.SalesOrderId);

            int rowId = 0;
            var fileNameGenerated = string.Format("{0}_{1}{2}", LookupKey.ReportFileName.SalesReport, DateTime.Now.ToString("MMddyyyy"), ".xlsx");

            var contentType = "application/vnd.ms-excel";

            //var templateFile = new FileInfo(path);
            //var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();
            var workSheet = package.Workbook.Worksheets.Add(LookupKey.ReportFileName.SalesReport);

            if (dt.Rows.Count > 0)
            {
                rowId = 1;
                workSheet.Cells[rowId, 1].Value = "Sales No";
                workSheet.Cells[rowId, 2].Value = dt.Rows[0]["SalesNo"].ToString();

                workSheet.Cells[rowId, 4].Value = "Mode of Payment";
                workSheet.Cells[rowId, 5].Value = dt.Rows[0]["ModeOfPayment"].ToString();

                rowId = 2;
                workSheet.Cells[rowId, 1].Value = "Customer";
                workSheet.Cells[rowId, 2].Value = dt.Rows[0]["CustomerFullDetails"].ToString();

                workSheet.Cells[rowId, 4].Value = "Address";
                workSheet.Cells[rowId, 5].Value = dt.Rows[0]["FullAddress"].ToString();


                rowId = 5;
                workSheet.Cells[rowId, 1].Value = "PRODUCT CODE";
                workSheet.Cells[rowId, 2].Value = "PRODUCT DESCRIPTION";
                workSheet.Cells[rowId, 3].Value = "SALES QUANTITY";
                workSheet.Cells[rowId, 4].Value = "PRICE";
                workSheet.Cells[rowId, 5].Value = "SUBTOTAL";



                rowId = rowId + 1;
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {

                    workSheet.Cells[rowId, 1].Value = dt.Rows[i]["ProductCode"].ToString();
                    workSheet.Cells[rowId, 2].Value = dt.Rows[i]["ProductDescription"].ToString();
                    workSheet.Cells[rowId, 3].Value = Convert.ToInt64(dt.Rows[i]["Quantity"].ToString());
                    workSheet.Cells[rowId, 4].Value = Convert.ToDecimal(dt.Rows[i]["UnitPrice"]).ToString("N");
                    workSheet.Cells[rowId, 5].Value = Convert.ToDecimal(dt.Rows[i]["Subtotal"]).ToString("N");
                    rowId = rowId + 1;
                }

                rowId = rowId + 2;
                workSheet.Cells[rowId, 4].Value = "Shipping Fee";
                workSheet.Cells[rowId, 5].Value = string.Format("{0:#,0.00}", Convert.ToDecimal(dt.Rows[0]["ShippingFee"]));
                rowId = rowId + 1;
                workSheet.Cells[rowId, 4].Value = "Total";
                workSheet.Cells[rowId, 5].Value = string.Format("{0:#,0.00}", Convert.ToDecimal(dt.Rows[0]["TotalAmount"]));

                workSheet.Cells.AutoFitColumns();

            }

            var memoryStream = new MemoryStream();
            //package.Save();
            package.SaveAs(memoryStream);
            memoryStream.Position = 0;

            return File(memoryStream, contentType, fileNameGenerated);
        }
        #endregion
    }
}