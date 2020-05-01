using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Extensions;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using dbentities = DataAccess.Database.InventoryAAA;
using DataAccess.Repository.InventoryAAA.Interface;

namespace Business.AAA.Core
{
    public partial class OrderServices
    {
        IInventoryAAARepository<dbentities.PurchaseOrder> _purchaseOrderServices;
        IInventoryAAARepository<dbentities.PurchaseOrderDetail> _purchaseOrderDetailServices;
        IInventoryAAARepository<dbentities.SalesOrder> _salesOrderServices;
        IInventoryAAARepository<dbentities.SalesOrderDetail> _salesOrderDetailServices;

        IProductServices _productServices;
        IOrderTypeServices _orderTypeServices;

        private dbentities.PurchaseOrder purchaseOrder;
        private dbentities.PurchaseOrderDetail purchaseOrderDetail;
        private dbentities.SalesOrder salesOrder;
        private dbentities.SalesOrderDetail salesOrderDetail;

        public OrderServices(IInventoryAAARepository<dbentities.PurchaseOrder> purchaseOrderServices,
        IInventoryAAARepository<dbentities.PurchaseOrderDetail> purchaseOrderDetailServices,
        IInventoryAAARepository<dbentities.SalesOrder> salesOrderServices,
        IInventoryAAARepository<dbentities.SalesOrderDetail> salesOrderDetailServices,

        IProductServices productServices,
        IOrderTypeServices orderTypeServices)
        {
            this._purchaseOrderServices = purchaseOrderServices;
            this._purchaseOrderDetailServices = purchaseOrderDetailServices;

            this._salesOrderServices = salesOrderServices;
            this._salesOrderDetailServices = salesOrderDetailServices;

            this._productServices = productServices;
            this._orderTypeServices = orderTypeServices;

            this.purchaseOrder = new dbentities.PurchaseOrder();
            this.purchaseOrderDetail = new dbentities.PurchaseOrderDetail();
            this.salesOrder = new dbentities.SalesOrder();
            this.salesOrderDetail = new dbentities.SalesOrderDetail();
        }
    }

    public partial class OrderServices : IOrderServices
    {

        public IQueryable<PurchaseOrderDetails> GetAllPurchaseOrderDetails()
        {
            var productResult = _productServices.GetAll();
            var purchaseOrderResult = GetAllPurchaseOrders();

            var result = from det in this._purchaseOrderDetailServices.GetAll()
                         select new PurchaseOrderDetails()
                         {
                             PurchaseOrderId = det.PurchaseOrderID,
                             ProductId = det.ProductID,
                             Quantity = det.Quantity,
                             UnitPrice = det.UnitPrice,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime,

                             ProductDetails = productResult.Where(p => p.ProductId == det.ProductID).FirstOrDefault(),
                             PurchaseOrders = purchaseOrderResult.Where(po => po.PurchaseOrderId == det.PurchaseOrderID).FirstOrDefault()
                         };

            return result;
        }

        public IQueryable<PurchaseOrders> GetAllPurchaseOrders()
        {
            var orderTypeResult = _orderTypeServices.GetAll();

            var result = from det in this._purchaseOrderServices.GetAll()
                         select new PurchaseOrders()
                         {
                             PurchaseOrderId = det.PurchaseOrderID,
                             PurchaseOrderTypeId = det.PurchaseOrderTypeID,
                             TotalQuantity = det.TotalQuantity,
                             TotalAmount = det.TotalAmount,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime,

                             OrderTypes = orderTypeResult.Where(ot => ot.OrderTypeId == det.PurchaseOrderTypeID).FirstOrDefault()
                         };

            return result;
        }

        public IQueryable<SalesOrderDetails> GetAllSalesOrderDetails()
        {
            var productResult = _productServices.GetAll();
            var salesOrderResult = GetAllSalesOrders();

            var result = from det in this._salesOrderDetailServices.GetAll()
                         select new SalesOrderDetails()
                         {
                             SalesOrderId = det.SalesOrderID,
                             ProductId = det.ProductID,
                             Quantity = det.Quantity,
                             UnitPrice = det.UnitPrice,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime,

                             ProductDetails = productResult.Where(p => p.ProductId == det.ProductID).FirstOrDefault(),
                             SalesOrders = salesOrderResult.Where(so => so.SalesOrderId == det.SalesOrderID).FirstOrDefault()
                         };

            return result;
        }

        public IQueryable<SalesOrders> GetAllSalesOrders()
        {
            var orderTypeResult = _orderTypeServices.GetAll();

            var result = from det in this._salesOrderServices.GetAll()
                         select new SalesOrders()
                         {
                             SalesOrderId = det.SalesOrderID,
                             SalesOrderTypeId = det.SalesOrderTypeID,
                             TotalQuantity = det.TotalQuantity,
                             TotalAmount = det.TotalAmount,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime,

                             OrderTypes = orderTypeResult.Where(ot => ot.OrderTypeId == det.SalesOrderTypeID).FirstOrDefault()
                         };

            return result;
        }

        public long SavePurchaseOrder(PurchaseOrdersRequest request)
        {
            request.PurchaseOrderId = 0;
            this.purchaseOrder = request.DtoToEntity();
            var item = this._purchaseOrderServices.Insert(this.purchaseOrder);
            if (item == null)
            {
                return 0;
            }

            return item.PurchaseOrderID;
        }

        public long SavePurchaseOrderDetails(PurchaseOrderDetailsRequest request)
        {
            this.purchaseOrderDetail = request.DtoToEntity();
            var item = this._purchaseOrderDetailServices.Insert(this.purchaseOrderDetail);
            if (item == null)
            {
                return 0;
            }

            return item.PurchaseOrderID;
        }

        public long SaveSalesOrder(SalesOrdersRequest request)
        {
            request.SalesOrderId = 0;
            this.salesOrder = request.DtoToEntity();
            var item = this._salesOrderServices.Insert(this.salesOrder);
            if (item == null)
            {
                return 0;
            }

            return item.SalesOrderID;
        }

        public long SaveSalesOrderDetails(SalesOrderDetailsRequest request)
        {
            this.salesOrderDetail = request.DtoToEntity();
            var item = this._salesOrderDetailServices.Insert(this.salesOrderDetail);
            if (item == null)
            {
                return 0;
            }

            return item.SalesOrderID;
        }

    }
}
