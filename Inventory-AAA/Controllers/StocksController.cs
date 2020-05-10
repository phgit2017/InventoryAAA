using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

using Business.AAA.Core;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;

namespace Inventory_AAA.Controllers
{
    public class StocksController : Controller
    {
        private readonly IOrderTypeServices _orderTypeServices;
        private readonly IOrderTransactionalServices _orderTransactionalServices;
        private readonly IProductServices _productServices;
        private readonly IOrderServices _orderServices;

        public StocksController(
            IOrderTypeServices orderTypeServices,
            IOrderTransactionalServices orderTransactionalServices,
            IProductServices productServices,
            IOrderServices orderServices)
        {
            this._orderTypeServices = orderTypeServices;
            this._orderTransactionalServices = orderTransactionalServices;
            this._productServices = productServices;
            this._orderServices = orderServices;
        }

        // GET: Stocks
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult InventorySummary()
        {
            var result = this._productServices.RetrieveInventorySummary();

            var response = new
            {
                result = result,
                isSuccess = true
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InventoryDetails(StocksDetailsRequest request)
        {

            //Product Details
            var productResult = _productServices.GetAll().Where(p => p.ProductId == p.ProductId).FirstOrDefault();

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

        [HttpPost]
        public JsonResult UpdateInventoryOrder(OrderRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty, orderTransactionTypeService = string.Empty;
            long updateOrderTransactionResult = 0;

            OrderTransactionRequest orderTransactionRequest = new OrderTransactionRequest();
            List<ProductDetailRequest> orderTransactionDetailRequest = new List<ProductDetailRequest>();

            #region Set Order Transaction Type

            
            if (request.OrderTransactionType == LookupKey.OrderTransactionType.PurchaseOrder)
            {
                orderTransactionTypeService = "PurchaseOrderService";
            }
            else if (request.OrderTransactionType == LookupKey.OrderTransactionType.SalesOrder)
            {
                orderTransactionTypeService = "SalesOrderService";
            }

            #endregion

            #region Service implementation

            
            //TODO:(LEP)Users put session
            orderTransactionRequest.CreatedBy = 0;

            orderTransactionDetailRequest.Add(new ProductDetailRequest()
            {
                ProductId = request.ProductId,
                ProductCode = request.ProductCode,
                ProductDescription = request.ProductDescription,
                Quantity = request.Stocks,
                UnitPrice = request.UnitPrice,
                IsActive = request.IsActive,
                CreatedBy = 0 //TODO:(LEP)Users put session

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

            if (updateOrderTransactionResult == 0)
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

        [HttpPost]
        public JsonResult UpdateProductDetails(ProductDetailRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty;
            bool productUpdateResult = false;

            //TODO:(LEP)Users put session
            request.CreatedBy = 0;

            //Update Product Details
            productUpdateResult = _productServices.UpdateDetails(request);

            if (!productUpdateResult)
            {
                return Json(new
                {
                    isSucess = isSucess,
                    messageAlert = "Server Error"
                }, JsonRequestBehavior.AllowGet);
            }


            var productLogDetailRequest = new ProductLogDetailRequest()
            {
                ProductLogsId = 0,
                ProductId = request.ProductId,
                ProductCode = request.ProductCode,
                ProductDescription = request.ProductDescription,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                IsActive = request.IsActive,
                CreatedBy = request.CreatedBy,
                CreatedTime = request.CreatedTime,
                ModifiedBy = request.ModifiedBy,
                ModifiedTime = DateTime.Now
            };
            var productLogResult = _productServices.SaveProductLogs(productLogDetailRequest);

            if (productLogResult <= 0)
            {
                return Json(new
                {
                    isSucess = isSucess,
                    messageAlert = "Server Error"
                }, JsonRequestBehavior.AllowGet);
            }

            isSucess = true;
            var response = new
            {
                isSucess = isSucess,
                messageAlert = messageAlert
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}