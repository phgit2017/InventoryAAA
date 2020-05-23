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

        [Display(Name = "Product Code")]
        [Required(ErrorMessage = "Product Code is required")]
        [StringLength(64, ErrorMessage = "Up to 64 characters only.")]
        public string ProductCode { get; set; }

        [Display(Name = "Product Description")]
        [StringLength(64, ErrorMessage = "Up to 64 characters only.")]
        public string ProductDescription { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public bool IsActive { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }
    
    public class ProductLogDetailRequest : BaseDetail
    {
        public long ProductLogsId { get; set; }

        public long ProductId { get; set; }

        [StringLength(32)]
        public string ProductCode { get; set; }

        [StringLength(32)]
        public string ProductDescription { get; set; } = string.Empty;

        public decimal? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public bool? IsActive { get; set; }


    }
}
