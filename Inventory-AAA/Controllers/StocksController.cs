using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

using Business.AAA.Core;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using Infrastructure.Utilities;
using Inventory_AAA.Infrastructure;
using Newtonsoft.Json;

namespace Inventory_AAA.Controllers
{
    public class StocksController : Controller
    {
        private readonly IOrderTypeServices _orderTypeServices;
        private readonly IOrderTransactionalServices _orderTransactionalServices;
        private readonly IProductServices _productServices;
        private readonly IOrderServices _orderServices;
        private readonly IUserServices _userServices;

        public StocksController(
            IOrderTypeServices orderTypeServices,
            IOrderTransactionalServices orderTransactionalServices,
            IProductServices productServices,
            IOrderServices orderServices,
            IUserServices userServices)
        {
            this._orderTypeServices = orderTypeServices;
            this._orderTransactionalServices = orderTransactionalServices;
            this._productServices = productServices;
            this._orderServices = orderServices;
            this._userServices = userServices;
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
        public JsonResult InventoryDetails(StocksDetailsSearchRequest request)
        {

            //Product Details
            var productResult = _productServices.GetAll().Where(p => p.ProductId == request.ProductId).FirstOrDefault();
            productResult.ProductPrices = _productServices.GetAllProductPrices().Where(m => m.ProductId == request.ProductId).ToList();

            //Purchase order and Sales order Details
            var inventoryDetailsResult = _productServices.RetrieveInventoryDetails(request);

            var response = new
            {
                ProductResult = productResult,
                InventoryDetailsResult = inventoryDetailsResult,
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
                else if(request.OrderTransactionType == LookupKey.OrderTransactionType.CorrectionOrder)
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
                    _orderServices);
                updateOrderTransactionResult = order.UpdateOrderTransaction(
                    orderTransactionRequest,
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
                ModifiedTime = null
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
    }
}