using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Business.AAA.Core;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;

namespace Inventory_AAA.Controllers
{
    public class CommonController : Controller
    {
        private readonly IProductServices _productServices;
        private readonly IOrderTypeServices _orderTypeServices;
        private readonly IUserServices _userServices;
        private readonly ICustomerServices _customerServices;

        public CommonController(
            IProductServices productServices,
            IOrderTypeServices orderTypeServices,
            IUserServices userServices,
            ICustomerServices customerServices)
        {
            this._productServices = productServices;
            this._orderTypeServices = orderTypeServices;
            this._userServices = userServices;
            this._customerServices = customerServices;
        }

        // GET: Common
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ProductList()
        {
            var result = _productServices.GetAll().Where(p => p.IsActive).ToList();
            var response = new
            {
                result = result,
                isSuccess = true
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OrderActionTypeList()
        {
            var result = new List<OrderTypeActionDetail>();

            result.Add(new OrderTypeActionDetail() { OrderId = 0, OrderActionName = "Purchase Order" });
            result.Add(new OrderTypeActionDetail() { OrderId = 1, OrderActionName = "Sales Order" });

            var response = new
            {
                result = result,
                isSuccess = true
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult MenuList()
        {
            
            var result = _userServices.GetAllMenuDetails().ToList();
            var response = new
            {
                result = result,
                isSuccess = true
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CustomerStatusList()
        {
            var result = _customerServices.GetAllCustomerStatus().ToList();
            var response = new
            {
                result = result,
                isSuccess = true
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}