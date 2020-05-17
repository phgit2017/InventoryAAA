using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Dto;

namespace Business.AAA.Core.Interface
{
    public interface IUserServices
    {
        IQueryable<UserDetail> GetAllUserDetails();
        IQueryable<UserRoleDetail> GetAllUserRoleDetails();
        long SaveUserDetails(UserDetailRequest request);
        bool UpdateUserDetails(UserDetailRequest request);
        UserDetail AuthenticateLogin(AuthenticateUserRequest request);
        IQueryable<Dto.UserMenuRoleDetail> GetAllUserMenuRoleDetails();
        IQueryable<Dto.MenuDetail> GetAllMenuDetails();
    }
}
