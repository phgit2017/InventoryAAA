using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class ReportDetail
    {
    }

    public class SalesReportRequest
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public long? ProductId { get; set; }
    }

    public class PurchaseAndReportDetail
    {
        public long ProductID { get; set; }
        public decimal PreviousPurchaseQty { get; set; }
        public decimal PurchaseQty { get; set; }
        public decimal PreviousSalesQty { get; set; }
        public decimal SalesQty { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CreatedBy { get; set; }
        public string Remarks { get; set; }
    }

    
}
