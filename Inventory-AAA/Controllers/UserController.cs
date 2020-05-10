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
            //TODO:(LEP)Users put session
            var currentUserId = 0;
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

            userIdResult = _userServices.SaveUserDetails(request);

            if (userIdResult != 0)
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
                isSuccess = isSucess,
                messageAlert = messageAlert
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUserDetails(UserDetailRequest request)
        {
            bool isSucess = false;
            string messageAlert = string.Empty;
            bool userIdResult = false;

            userIdResult = _userServices.UpdateUserDetails(request);

            if (userIdResult)
            {
                return Json(new
                {
                    isSucess = isSucess,
                    messageAlert = "Server Error"
                }, JsonRequestBehavior.AllowGet);
            }

            var response = new
            {
                isSuccess = isSucess,
                messageAlert = messageAlert
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AuthenticateLogin(AuthenticateUserRequest request)
        {
            bool isSuccess = false;
            var authenticateLoginResult = _userServices.AuthenticateLogin(request);
            
            if (authenticateLoginResult != null)
            {
                isSuccess = true;
            }

            Session[LookupKey.SessionVariables.UserId] = authenticateLoginResult.UserId;

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