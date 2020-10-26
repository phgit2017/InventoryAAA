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
                    isSuccess = true,
                    messageAlert = authorizeMenuAccessResult.MessageAlert,
                    CustomerDetailsResult = customerDetailResult
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);
            customerDetailResult = _customerServices.GetAll().ToList();
            var response = new
            {
                CustomerDetailsResult = customerDetailResult,
                isSuccess = true,
                messageAlert = ""
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddNewCustomerDetails(CustomerDetailRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty;
            long customerIdResult = 0;

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);

            request.CreatedBy = currentUserId;
            request.CreatedTime = DateTime.Now;

            if (ModelState.IsValid)
            {

                customerIdResult = _customerServices.SaveDetail(request);

                if (customerIdResult == -100)
                {
                    return Json(new { isSuccess = isSucess, messageAlert = Messages.CustomerNameValidation }, JsonRequestBehavior.AllowGet);
                }
                if (customerIdResult == 0)
                {
                    return Json(new { isSuccess = isSucess, messageAlert = Messages.ServerError }, JsonRequestBehavior.AllowGet);
                }

                isSucess = true;
                var response = new
                {
                    isSuccess = isSucess,
                    messageAlert = messageAlert
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { isSuccess = isSucess, messageAlert = Messages.ErrorOccuredDuringProcessing }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult UpdateCustomerDetails(CustomerDetailRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty;
            bool customerIdResult = false;

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);
            var passedUserResult = _customerServices.GetAll().Where(m => m.CustomerId == request.CustomerId).FirstOrDefault();

            request.CreatedTime = passedUserResult.CreatedTime;
            request.ModifiedBy = currentUserId;
            request.ModifiedTime = DateTime.Now;

            #region Validate same firstname and lastname
            var customerNameResult = _customerServices.GetAll().Where(u => u.FirstName == request.FirstName
                                                           && u.CustomerStatusId == LookupKey.CustomerStatus.ActiveId
                                                           && u.LastName == request.LastName
                                                           && u.CustomerId != request.CustomerId).FirstOrDefault();


            if (!customerNameResult.IsNull())
            {
                return Json(new { isSuccess = isSucess, messageAlert = Messages.CustomerNameValidation }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            customerIdResult = _customerServices.UpdateDetail(request);

            if (!customerIdResult)
            {
                return Json(new { isSuccess = isSucess, messageAlert = Messages.ServerError }, JsonRequestBehavior.AllowGet);
            }

            isSucess = true;
            var response = new
            {
                isSuccess = isSucess,
                messageAlert = messageAlert
            };

            return Json(response, JsonRequestBehavior.AllowGet);


        }
        #endregion

        #region Category
        [HttpGet]
        public JsonResult CategoryList()
        {
            List<CategoryDetail> categoryDetailResult = new List<CategoryDetail>();

            #region Authorize
            var authorizeMenuAccessResult = AuthorizeMenuAccess(LookupKey.Menu.CustomerMenuId);
            if (!authorizeMenuAccessResult.IsSuccess)
            {

                return Json(new
                {
                    isSuccess = authorizeMenuAccessResult.IsSuccess,
                    messageAlert = authorizeMenuAccessResult.MessageAlert,
                    CategoryDetailsResult = categoryDetailResult
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);
            categoryDetailResult = _categoryServices.GetAll().ToList();
            var response = new
            {
                CategoryDetailsResult = categoryDetailResult,
                isSuccess = true,
                messageAlert = ""
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddNewCategoryDetails(CategoryDetailRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty;
            long categoryIdResult = 0;
            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);

            request.CreatedBy = currentUserId;
            request.CreatedTime = DateTime.Now;

            if (ModelState.IsValid)
            {

                categoryIdResult = _categoryServices.SaveDetail(request);

                if (categoryIdResult == -100)
                {
                    return Json(new { isSuccess = isSucess, messageAlert = Messages.CategoryNameValidation, CategoryId = categoryIdResult }, JsonRequestBehavior.AllowGet);
                }
                if (categoryIdResult == 0)
                {
                    return Json(new { isSuccess = isSucess, messageAlert = Messages.ServerError,CategoryId = categoryIdResult }, JsonRequestBehavior.AllowGet);
                }

                isSucess = true;
                var response = new
                {
                    isSuccess = isSucess,
                    messageAlert = messageAlert,
                    CategoryId = categoryIdResult
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { isSuccess = isSucess, messageAlert = Messages.ErrorOccuredDuringProcessing, CategoryId = categoryIdResult }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult UpdateCategoryDetails(CategoryDetailRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty;
            bool categoryIdResult = false;

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);
            var passedUserResult = _categoryServices.GetAll().Where(m => m.CategoryId == request.CategoryId).FirstOrDefault();

            request.CreatedTime = passedUserResult.CreatedTime;
            request.ModifiedBy = currentUserId;
            request.ModifiedTime = DateTime.Now;

            
            #region Validate same category name

            var categoryNameResult = _categoryServices.GetAll().Where(u => u.CategoryName == request.CategoryName
                                                          && u.IsActive).FirstOrDefault();
            if (!categoryNameResult.IsNull())
            {
                return Json(new { isSuccess = isSucess, messageAlert = Messages.CategoryNameValidation }, JsonRequestBehavior.AllowGet);
            }
            #endregion

            categoryIdResult = _categoryServices.UpdateDetail(request);

            if (!categoryIdResult)
            {
                return Json(new { isSuccess = isSucess, messageAlert = Messages.ServerError }, JsonRequestBehavior.AllowGet);
            }

            isSucess = true;
            var response = new
            {
                isSuccess = isSucess,
                messageAlert = messageAlert
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