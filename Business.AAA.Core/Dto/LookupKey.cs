using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public static class LookupKey
    {
        public static class OrderType
        {
            public const int Single = 1;
            public const int Batch = 2;
        }

        public static class OrderTransactionType
        {
            public const int PurchaseOrder = 0;
            public const int SalesOrder = 1;
        }

        public static class SessionVariables
        {
            public const string UserId = "UserId";
            public const string UserFullName = "UserFullName";
            public const string UserRoleName = "UserRoleName";
        }

        public static class ReportFileName
        {
            public const string SalesReport = "SalesReport";
            public const string PurchaseAndSalesReport = "PurchaseAndSalesReport";
        }

        public static class Menu
        {
            public const int InventoryMenuId = 1;
            public const int UserMenuId= 2;
            public const int ReportMenuId = 3;
            public const int CustomerMenuId = 4;
        }
    }

    public static class Messages
    {
        public const string ProductCodeValidation = "Duplicate product code.";
        public const string UserNameValidation = "Duplicate username.";
        public const string ErrorOccuredDuringProcessing = "An error occured during the process. Please check the details, refresh the page, and try again.";
        public const string ServerError = "Server Error";
        public const string SessionUnavailable = "Session is unavailable";
        public const string UnauthorizeAccess = "Unauthorized access";
    }
}
