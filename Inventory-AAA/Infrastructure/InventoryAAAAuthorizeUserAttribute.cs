using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Business.AAA.Core;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;

namespace Inventory_AAA.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InventoryAAAAuthorizeUserAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;

        public InventoryAAAAuthorizeUserAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            HttpContext ctx = HttpContext.Current;

            
            long userId = Convert.ToInt64(ctx.Session[LookupKey.SessionVariables.UserId]);
            bool authorize = false;

            foreach (var role in allowedroles)
            {

                //var user = _userServices.GetAllUserDetails().Where(m => m.UserId == userId && m.UserRoleDetails.UserRoleName == role && m.IsActive);
                //if (user.Count() > 0)
                //{
                //    authorize = true;
                //}
            }


            return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/User/Index");
            //filterContext.Result = new HttpUnauthorizedResult();
        }

    }
}