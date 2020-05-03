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
        public long ProductId { get; set; }

        [Required]
        public string ProductCode { get; set; }

        public string ProductDescription { get; set; }

        public decimal Stocks { get; set; }

        public decimal UnitPrice { get; set; }

        public int OrderTransactionType { get; set; }

        public bool IsActive { get; set; }

    }

    public class OrderTransactionRequest : BaseDetail
    {
        public long OrderId { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public int OrderTypeId { get; set; }
    }

    public class OrderTransactionDetailRequest : BaseDetail
    {
        public long PurchaseOrderId { get; set; }

        public long ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        
    }


}
