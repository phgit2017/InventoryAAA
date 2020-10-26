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

        public string SalesNo { get; set; }

        public int SalesOrderStatusId { get; set; }

        public string SalesOrderStatusName { get; set; }

        public long CustomerId { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string CustomerFullAddress { get; set; }

        public string CustomerFullName
        {
            get
            {
                return CustomerFirstName + " " + CustomerLastName;
            }
        }

        public string ModeOfPayment { get; set; }

        public decimal? ShippingFee { get; set; }


    }

    public class BaseOrders : BaseDetail
    {
        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderTypeDetail OrderTypes { get; set; }
    }

    public class SalesOrderReceiptDetail
    {
        public string SalesNo { get; set; }
        public long SalesOrderId { get; set; }
        public long SalesOrderStatusID { get; set; }
        public string SalesOrderStatusDisplay { get; set; }
        public long ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public long CategoryID { get; set; }
        public string CategoryName { get; set; }
        public decimal PreviousQuantity { get; set; }
        public long Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public long CreatedBy { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserFullName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal Subtotal { get; set; }
    }
    #endregion

    #region OrdersRequest
    public class CorrectionOrdersRequest : BaseOrdersRequest
    {
        public long CorrectionOrderId { get; set; }
        public int CorrectionOrderTypeId { get; set; }

    }
    public class PurchaseOrdersRequest : BaseOrdersRequest
    {
        public long PurchaseOrderId { get; set; }


        public int PurchaseOrderTypeId { get; set; }
    }



    public class SalesOrdersRequest : BaseOrdersRequest
    {
        public long SalesOrderId { get; set; }

        public int SalesOrderTypeId { get; set; }

        public string SalesNo { get; set; }

        public long CustomerId { get; set; }

        public int SalesOrderStatusId { get; set; } = 1;

        public string ModeOfPayment { get; set; }

        public decimal? ShippingFee { get; set; }
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
        

    }

    public class SalesOrderDetails : BaseOrderDetails
    {
        public long SalesOrderId { get; set; }

        public decimal CurrentStock { get; set; } = 0;

        public int PriceTypeId { get; set; }
        public string PriceTypeName { get; set; }
    }

    public class BaseOrderDetails : BaseDetail
    {
        public long ProductId { get; set; }

        public string ProductCode { get; set; }

        public string ProductDescription { get; set; }

        public long? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public decimal? PreviousQuantity { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
    #endregion

    #region OrdersDetailsRequest
    public class CorrectionOrderDetailsRequest : BaseOrderRequestDetails
    {

        public long CorrectionOrderId { get; set; }

    }
    public class PurchaseOrderDetailsRequest : BaseOrderRequestDetails
    {

        public long PurchaseOrderId { get; set; }

    }

    public class SalesOrderDetailsRequest : BaseOrderRequestDetails
    {
        public long SalesOrderId { get; set; }

        public int PriceTypeId { get; set; }
    }

    public class BaseOrderRequestDetails : BaseDetail
    {
        public long ProductId { get; set; }

        public decimal? PreviousQuantity { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }
    #endregion




}
