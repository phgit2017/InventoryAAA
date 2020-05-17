using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class OrderDetail
    {
    }

    #region Orders
    public class PurchaseOrders : BaseOrders
    {
        public long PurchaseOrderId { get; set; }

        public int PurchaseOrderTypeId { get; set; }

    }

    public class SalesOrders : BaseOrders
    {
        public long SalesOrderId { get; set; }

        public int SalesOrderTypeId { get; set; }


    }

    public class BaseOrders : BaseDetail
    {
        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderTypeDetail OrderTypes { get; set; }
    }
    #endregion

    #region OrdersRequest
    public class PurchaseOrdersRequest : BaseOrdersRequest
    {
        public long PurchaseOrderId { get; set; }


        public int PurchaseOrderTypeId { get; set; }
    }



    public class SalesOrdersRequest : BaseOrdersRequest
    {
        public long SalesOrderId { get; set; }

        public int SalesOrderTypeId { get; set; }
    }


    public class BaseOrdersRequest : BaseDetail
    {
        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }
    }
    #endregion

    #region OrdersDetails
    public class PurchaseOrderDetails : BaseOrderDetails
    {

        public long PurchaseOrderId { get; set; }

        public PurchaseOrders PurchaseOrders { get; set; }

    }

    public class SalesOrderDetails : BaseOrderDetails
    {
        public long SalesOrderId { get; set; }

        public SalesOrders SalesOrders { get; set; }
    }

    public class BaseOrderDetails : BaseDetail
    {
        public long ProductId { get; set; }

        public decimal? PreviousQuantity { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public ProductDetail ProductDetails { get; set; }
    }
    #endregion

    #region OrdersDetailsRequest
    public class PurchaseOrderDetailsRequest : BaseOrderRequestDetails
    {

        public long PurchaseOrderId { get; set; }

    }

    public class SalesOrderDetailsRequest : BaseOrderRequestDetails
    {
        public long SalesOrderId { get; set; }
    }

    public class BaseOrderRequestDetails : BaseDetail
    {
        public long ProductId { get; set; }

        public decimal? PreviousQuantity { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
    #endregion




}
