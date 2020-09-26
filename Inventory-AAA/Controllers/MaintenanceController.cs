using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MaintenanceController : Controller
    {
        private IUserServices _userServices;
        private ICategoryServices _categoryServices;
        private ICustomerServices _customerServices;

        public MaintenanceController(
            IUserServices userServices,
            ICategoryServices categoryServices,
            ICustomerServices customerServices)
        {
            this._userServices = userServices;
            this._categoryServices = categoryServices;
            this._customerServices = customerServices;
        }

        // GET: Maintenance
        public ActionResult Index()
        {
            return View();
        }

        #region Customer
        [HttpGet]
        public JsonResult CustomerList()
        {
            List<CustomerDetail> customerDetailResult = new List<CustomerDetail>();

            #region Authorize
            var authorizeMenuAccessResult = AuthorizeMenuAccess(LookupKey.Menu.CustomerMenuId);
            if (!authorizeMenuAccessResult.IsSuccess)
            {

                return Json(new
                {
                    isSuccess = authorizeMenuAccessResult.IsSuccess,
                    messageAlert = authorizeMenuAccessResult.MessageAlert,
                    UserDetailsResult = customerDetailResult
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);
            customerDetailResult = _customerServices.GetAll().ToList();
            var response = new
            {
                UserDetailsResult = customerDetailResult,
                isSuccess = true,
                messageAlert = ""
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        #endregion


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