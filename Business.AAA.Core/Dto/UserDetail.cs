﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class UserDetail : BaseDetail
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int UserRoleId { get; set; }

        public bool IsActive { get; set; }

        public UserRoleDetail UserRoleDetails { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

    }

    public class UserMenuRoleDetail
    {
        public int MenuId { get; set; }

        public int RoleId { get; set; }

    }

    public class MenuDetail
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
    }

    public class AuthorizationDetail
    {
        public bool IsSuccess { get; set; } = false;
        public string MessageAlert { get; set; } = string.Empty;
    }

    public class AuthenticateUserRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class UserDetailSearchRequest
    {
        public string UserName { get; set; }

        public int UserRoleId { get; set; }

        public bool IsActive { get; set; }

    }

    public class UserDetailRequest : BaseDetail
    {
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "UserName is required")]
        [StringLength(64, ErrorMessage = "Up to 64 characters only.")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(64, ErrorMessage = "Up to 64 characters only.")]
        public string Password { get; set; }

        [Required]
        public int UserRoleId { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(32, ErrorMessage = "Up to 32 characters only.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(32, ErrorMessage = "Up to 32 characters only.")]
        public string LastName { get; set; }

    }

    public class UserRoleDetail : BaseDetail
    {
        public int UserRoleId { get; set; }

        public string UserRoleName { get; set; }
    }

    public class UserInformationDetail
    {
        [StringLength(32)]
        public string FirstName { get; set; }

        [StringLength(32)]
        public string LastName { get; set; }
    }

    public class UserInformationDetailRequest
    {
        public int UserId { get; set; }

        [StringLength(32)]
        public string FirstName { get; set; }

        [StringLength(32)]
        public string LastName { get; set; }
    }
}
