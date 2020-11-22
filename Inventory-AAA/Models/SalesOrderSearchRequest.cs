using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inventory_AAA.Models
{
    public class SalesOrderSearchRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SalesOrderStatusId { get; set; }
    }
}