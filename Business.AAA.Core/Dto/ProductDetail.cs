using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class ProductDetail : BaseDetail
    {
        public long ProductId { get; set; }

        [StringLength(32)]
        public string ProductCode { get; set; }

        [StringLength(32)]
        public string ProductDescription { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsActive { get; set; }
    }

    public class ProductDetailRequest : BaseDetail
    {
        public long ProductId { get; set; }

        [StringLength(32)]
        public string ProductCode { get; set; }

        [StringLength(32)]
        public string ProductDescription { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsActive { get; set; }
    }
    
    public class ProductLogDetailRequest : BaseDetail
    {
        public long ProductLogsId { get; set; }

        public long ProductId { get; set; }

        [StringLength(32)]
        public string ProductCode { get; set; }

        [StringLength(32)]
        public string ProductDescription { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public bool? IsActive { get; set; }


    }
}
