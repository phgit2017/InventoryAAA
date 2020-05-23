using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Dto;
using dbentities = DataAccess.Database.InventoryAAA;

namespace Business.AAA.Core.Extensions
{
    public static class EntityMapper
    {
        public static dbentities.Product DtoToEntity(this ProductDetailRequest request)
        {
            dbentities.Product entity = null;

            if (request != null)
            {
                entity = new dbentities.Product
                {
                    ProductID = request.ProductId,
                    ProductCode = request.ProductCode,
                    IsActive = request.IsActive,
                    CreatedBy = request.CreatedBy,
                    CreatedTime = request.CreatedTime,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedTime = request.ModifiedTime,
                    ProductDescription = request.ProductDescription,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice
                };
            }

            return entity;
        }

        public static dbentities.ProductLog DtoToEntity(this ProductLogDetailRequest request)
        {
            dbentities.ProductLog entity = null;

            if (request != null)
            {
                entity = new dbentities.ProductLog
                {
                    ProductID = request.ProductId,
                    ProductLogsID = request.ProductLogsId,
                    ProductCode = request.ProductCode,
                    CreatedBy = request.CreatedBy,
                    CreatedTime = request.CreatedTime,
                    ProductDescription = request.ProductDescription,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    IsActive = request.IsActive,
                };
            }

            return entity;
        }

        public static dbentities.PurchaseOrder DtoToEntity(this PurchaseOrdersRequest request)
        {
            dbentities.PurchaseOrder entity = null;

            if (request != null)
            {
                entity = new dbentities.PurchaseOrder
                {
                    PurchaseOrderID = request.PurchaseOrderId,
                    OrderTypeID = request.PurchaseOrderTypeId,
                    TotalQuantity = request.TotalQuantity,
                    TotalAmount = request.TotalAmount,
                    CreatedBy = request.CreatedBy,
                    CreatedTime = request.CreatedTime,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedTime = request.ModifiedTime
                };
            }

            return entity;
        }

        public static dbentities.PurchaseOrderDetail DtoToEntity(this PurchaseOrderDetailsRequest request)
        {
            dbentities.PurchaseOrderDetail entity = null;

            if (request != null)
            {
                entity = new dbentities.PurchaseOrderDetail
                {
                    PurchaseOrderID = request.PurchaseOrderId,
                    ProductID = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    CreatedBy = request.CreatedBy,
                    CreatedTime = request.CreatedTime,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedTime = request.ModifiedTime,
                    PreviousQuantity = request.PreviousQuantity,
                    Remarks = request.Remarks
                    
                };
            }

            return entity;
        }

        public static dbentities.SalesOrderDetail DtoToEntity(this SalesOrderDetailsRequest request)
        {
            dbentities.SalesOrderDetail entity = null;

            if (request != null)
            {
                entity = new dbentities.SalesOrderDetail
                {
                    SalesOrderID = request.SalesOrderId,
                    ProductID = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    CreatedBy = request.CreatedBy,
                    CreatedTime = request.CreatedTime,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedTime = request.ModifiedTime,
                    PreviousQuantity = request.PreviousQuantity,
                    Remarks = request.Remarks
                };
            }

            return entity;
        }

        public static dbentities.SalesOrder DtoToEntity(this SalesOrdersRequest request)
        {
            dbentities.SalesOrder entity = null;

            if (request != null)
            {
                entity = new dbentities.SalesOrder
                {
                    SalesOrderID = request.SalesOrderId,
                    OrderTypeID = request.SalesOrderTypeId,
                    TotalQuantity = request.TotalQuantity,
                    TotalAmount = request.TotalAmount,
                    CreatedBy = request.CreatedBy,
                    CreatedTime = request.CreatedTime,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedTime = request.ModifiedTime
                };
            }

            return entity;
        }

        public static dbentities.UserDetail DtoToEntity(this UserDetailRequest request)
        {
            dbentities.UserDetail entity = null;

            if (request != null)
            {
                entity = new dbentities.UserDetail
                {
                    UserID = request.UserId,
                    UserName = request.UserName,
                    Password = request.Password,
                    UserRoleID = request.UserRoleId,
                    IsActive = request.IsActive,
                    CreatedBy = request.CreatedBy,
                    CreatedTime = request.CreatedTime,
                    ModifiedBy = request.ModifiedBy,
                    ModifiedTime = request.ModifiedTime
                };
            }

            return entity;
        }

        public static dbentities.UserInformationDetail DtoToEntity(this UserInformationDetailRequest request)
        {
            dbentities.UserInformationDetail entity = null;

            if (request != null)
            {
                entity = new dbentities.UserInformationDetail
                {
                    UserID = request.UserId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                };
            }

            return entity;
        }
    }
}
