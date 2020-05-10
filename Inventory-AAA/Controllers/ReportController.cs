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
    public class ReportController : Controller
    {
        private readonly IProductServices _productServices;
        public ReportController(
            IProductServices productServices)
        {
            this._productServices = productServices;
        }

        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
    }
}