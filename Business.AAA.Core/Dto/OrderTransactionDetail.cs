using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class OrderTransactionDetail
    {
    }

    public class OrderTransactionRequest : BaseDetail
    {
        public long OrderID { get; set; }

        public decimal TotalQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public int OrderTypeID { get; set; }
    }

    public class OrderTransactionDetailRequest : BaseDetail
    {
        public long PurchaseOrderId { get; set; }

        public long ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        
    }


}
