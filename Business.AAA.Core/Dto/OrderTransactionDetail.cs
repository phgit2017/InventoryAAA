using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class OrderRequest
    {
        public OrderRequest()
        {
            ProductPrices = new List<ProductPricesDetailRequest>();
        }

        public long ProductId { get; set; }

        [Display(Name = "Product Code")]
        [Required(ErrorMessage = "Product Code is required")]
        [StringLength(64, ErrorMessage = "Up to 64 characters only.")]
        public string ProductCode { get; set; }

        public string ProductDescription { get; set; } = string.Empty;

        [Display(Name = "Stocks")]
        [Required(ErrorMessage = "Stocks is required")]
        public decimal Stocks { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit Price is required")]
        public decimal UnitPrice { get; set; }

        public long? CategoryId { get; set; }

        public int OrderTransactionType { get; set; }

        public bool IsActive { get; set; }

        public string Remarks { get; set; } = string.Empty;

        public List<ProductPricesDetailRequest> ProductPrices { get; set; }

        public decimal RetailerPrice { get; set; } = 0;
        public decimal ResellerPrice { get; set; } = 0;
        public decimal BigBuyerPrice { get; set; } = 0;

    }

    public class SalesOrderRequest
    {
        public SalesOrderRequest()
        {
            SalesOrderProductDetailRequest = new List<SalesOrderProductDetailRequest>();
        }



        public int OrderTransactionType { get; set; }

        public long CustomerId { get; set; }

        public List<SalesOrderProductDetailRequest> SalesOrderProductDetailRequest { get; set; }

        public long SalesOrderId { get; set; }

        public int SalesOrderStatusId { get; set; }

        public string SalesNo { get; set; }

        public string ModeOfPayment { get; set; }
        public decimal? ShippingFee { get; set; } 

    }

    public class SalesOrderProductDetailRequest
    {
        public long ProductId { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

    }

    public partial class OrderTransactionRequest : BaseDetail
    {
        public long OrderId { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public int OrderTypeId { get; set; }
        
    }
    #region Sales Order

    /// <summary>
    /// Sales Order
    /// </summary>
    public partial class OrderTransactionRequest
    {
        public long CustomerId { get; set; }

        public int SalesOrderStatusId { get; set; }

        public long SalesOrderId { get; set; }

        public string SalesNo { get; set; }
        public string ModeOfPayment { get; set; }

        public decimal? ShippingFee { get; set; }
    }

    #endregion

    public class OrderTransactionDetailRequest : BaseDetail
    {
        public long PurchaseOrderId { get; set; }

        public long ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

    }


}
