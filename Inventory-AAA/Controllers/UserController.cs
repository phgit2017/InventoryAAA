using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Business.AAA.Core;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using Infrastructure.Utilities;

namespace Inventory_AAA.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserServices _userServices;

        public UserController(
            IUserServices userServices)
        {
            this._userServices = userServices;
        }

        [HttpPost]
        public JsonResult UserList(UserDetailSearchRequest request)
        {
            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);
            var userDetailsResult = _userServices.GetAllUserDetails().Where(m => m.UserId != currentUserId).ToList();
            var response = new
            {
                UserDetailsResult = userDetailsResult,
                isSuccess = true
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddNewUserDetails(UserDetailRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty;
            long userIdResult = 0;

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);

            request.CreatedBy = currentUserId;
            request.CreatedTime = DateTime.Now;

            if (ModelState.IsValid)
            {

                userIdResult = _userServices.SaveUserDetails(request);

                if (userIdResult == -100)
                {
                    return Json(new { isSucess = isSucess, messageAlert = Messages.UserNameValidation }, JsonRequestBehavior.AllowGet);
                }
                if (userIdResult == 0)
                {
                    return Json(new { isSucess = isSucess, messageAlert = Messages.ServerError }, JsonRequestBehavior.AllowGet);
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
                return Json(new { isSucess = isSucess, messageAlert = Messages.ErrorOccuredDuringProcessing }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult UpdateUserDetails(UserDetailRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty;
            bool userIdResult = false;

            var currentUserId = Session[LookupKey.SessionVariables.UserId].IsNull() ? 0 : Convert.ToInt64(Session[LookupKey.SessionVariables.UserId]);

            request.ModifiedBy = currentUserId;
            request.ModifiedTime = DateTime.Now;

            if (ModelState.IsValid)
            {
                var codeUserDetailResult = _userServices.GetAllUserDetails().Where(u => u.UserName == request.UserName
                                                                     && u.UserId != request.UserId
                                                                               && u.IsActive).FirstOrDefault();

                #region Validate same username
                if (!codeUserDetailResult.IsNull())
                {
                    return Json(new { isSucess = isSucess, messageAlert = Messages.UserNameValidation }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                userIdResult = _userServices.UpdateUserDetails(request);

                if (!userIdResult)
                {
                    return Json(new { isSucess = isSucess, messageAlert = Messages.ServerError }, JsonRequestBehavior.AllowGet);
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
                return Json(new { isSucess = isSucess, messageAlert = Messages.ErrorOccuredDuringProcessing }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult AuthenticateLogin(AuthenticateUserRequest request)
        {
            bool isSuccess = false;
            var authenticateLoginResult = _userServices.AuthenticateLogin(request);

            if (authenticateLoginResult == null)
            {
                return Json(new
                {
                    isSucess = isSuccess,
                    messageAlert = "Invalid User Details"
                }, JsonRequestBehavior.AllowGet);
            }

            isSuccess = true;
            Session[LookupKey.SessionVariables.UserId] = authenticateLoginResult.UserId;

            HttpCookie userFullName = new HttpCookie("UserFullName",authenticateLoginResult.FullName);
            HttpCookie userRoleName = new HttpCookie("UserRoleName", authenticateLoginResult.UserRoleDetails.UserRoleName);
            Response.Cookies.Add(userFullName);
            Response.Cookies.Add(userRoleName);

            var response = new
            {
                userDetailResult = authenticateLoginResult,
                isSuccess = isSuccess
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Logout()
        {
            Response.Cookies.Remove("UserFullName");
            Response.Cookies.Remove("UserRoleName");
            
            Session.Abandon();
            
            var response = new
            {
                isSuccess = true,
                messageAlert = string.Empty

            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}