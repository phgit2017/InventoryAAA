using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class InventoryDetail
    {
    }

    public class StocksSummary
    {
        public long ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public decimal InventoryStocks { get; set; }
        public decimal CurrentStocks { get; set; }
        public decimal BigBuyerPrice { get; set; }
        public decimal ResellerPrice { get; set; }
        public decimal RetailerPrice { get; set; }
        public string CategoryName { get; set; }
        public long CategoryId { get; set; }
        public decimal Sold { get; set; }
    }

    public class StocksDetails
    {
        public long ProductID { get; set; }
        public decimal PurchaseQty { get; set; }
        public decimal SalesQty { get; set; }
        public decimal CurrentStocks { get; set; }
        public DateTime TransactionDate { get; set; }

        public string TransactionDateTimeFormat
        {
            get
            {
                if (TransactionDate != null)
                {
                    return TransactionDate.ToString("MM/dd/yyyy HH:mm:ss");
                }
                else
                {
                    return "";
                }
            }
        }

        public string TransactionDateFormat
        {
            get
            {
                if (TransactionDate != null)
                {
                    return TransactionDate.ToString("MM/dd/yyyy");
                }
                else
                {
                    return "";
                }
            }
        }
    }

    public class StocksDetailsSearchRequest
    {
        public long ProductId { get; set; }
    }
}
