using Business.AAA.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory_AAA.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InventoryAAAAuthorizeUserAttribute : AuthorizeAttribute
    {
        

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
            //{
            //    {"action","Index" },
            //    {"controller","Home" }
            //});

            //filterContext.Result = new ViewResult
            //{
            //    ViewName = "~/Views/Unauthorized.cshtml"
            //};

            filterContext.Result = new RedirectResult("~/Views/Unauthorized.cshtml");
            //filterContext.Result = new RedirectResult("~/#/Login");
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }

            //filterContext.RequestContext.HttpContext.Response.Redirect("/#/Login", true);
        }
    }
}