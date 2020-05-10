using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Extensions;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using dbentities = DataAccess.Database.InventoryAAA;
using DataAccess.Database.InventoryAAA;
using DataAccess.Repository.InventoryAAA.Interface;
using Infrastructure.Utilities;
using System.Data.SqlClient;
using System.Data;

namespace Business.AAA.Core
{
    public partial class UserServices
    {
        IInventoryAAARepository<dbentities.UserDetail> _userDetailServices;
        IInventoryAAARepository<dbentities.UserRoleDetail> _userRoleDetailServices;
        IInventoryAAARepository<dbentities.UserInformationDetail> _userInformationDetailServices;

        private dbentities.UserDetail userDetails;
        private dbentities.UserRoleDetail userRoleDetails;
        private dbentities.UserInformationDetail userInformationDetails;

        public UserServices(IInventoryAAARepository<dbentities.UserDetail> userDetailServices,
            IInventoryAAARepository<dbentities.UserRoleDetail> userRoleDetailServices,
            IInventoryAAARepository<dbentities.UserInformationDetail> userInformationDetailServices)
        {
            this._userDetailServices = userDetailServices;
            this._userRoleDetailServices = userRoleDetailServices;
            this._userInformationDetailServices = userInformationDetailServices;

            this.userDetails = new dbentities.UserDetail();
            this.userRoleDetails = new dbentities.UserRoleDetail();
            this.userInformationDetails = new dbentities.UserInformationDetail();
        }
    }

    public partial class UserServices : IUserServices
    {
        public IQueryable<Dto.UserDetail> GetAllUserDetails()
        {
            var userRoleResult = GetAllUserRoleDetails();
            var result = from det in this._userDetailServices.GetAll()
                         select new Dto.UserDetail()
                         {
                             UserId = det.UserID,
                             UserName = det.UserName,
                             IsActive = det.IsActive,
                             Password = det.Password,
                             UserRoleId = det.UserRoleID,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime,

                             UserRoleDetails = userRoleResult.Where(m => m.UserRoleId == det.UserRoleID).FirstOrDefault()
                         };

            return result;
        }

        public IQueryable<Dto.UserRoleDetail> GetAllUserRoleDetails()
        {
            var result = from det in this._userRoleDetailServices.GetAll()
                         select new Dto.UserRoleDetail()
                         {
                             UserRoleId = det.UserRoleID,
                             UserRoleName = det.UserRoleName,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime
                         };

            return result;
        }

        public long SaveUserDetails(UserDetailRequest request)
        {
            request.UserId = 0;
            this.userDetails = request.DtoToEntity();
            var item = this._userDetailServices.Insert(this.userDetails);
            if (item == null)
            {
                return 0;
            }

            UserInformationDetailRequest userInformationDetailRequest = new UserInformationDetailRequest()
            {
                UserId = item.UserID,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            this.userInformationDetails = userInformationDetailRequest.DtoToEntity();
            var itemUserInformation = this._userInformationDetailServices.Insert(this.userInformationDetails);

            if (itemUserInformation == null)
            {
                return 0;
            }

            return item.UserID;
        }

        public bool UpdateUserDetails(UserDetailRequest request)
        {
            this.userDetails = request.DtoToEntity();
            var item = _userDetailServices.Update2(this.userDetails);
            if (item == null)
            {
                return false;
            }

            UserInformationDetailRequest userInformationDetailRequest = new UserInformationDetailRequest()
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            this.userInformationDetails = userInformationDetailRequest.DtoToEntity();
            var itemUserInformation = this._userInformationDetailServices.Insert(this.userInformationDetails);

            if (itemUserInformation == null)
            {
                return false;
            }

            return true;
        }

        public Dto.UserDetail AuthenticateLogin(AuthenticateUserRequest request)
        {
            Dto.UserDetail result = new Dto.UserDetail();

            result = GetAllUserDetails().Where(m => m.UserName == request.UserName
                                                    && m.Password == request.Password
                                                    && m.IsActive).FirstOrDefault();

            if (result.IsNull())
            {
                return null;
            }

            return result;
        }
    }
}
