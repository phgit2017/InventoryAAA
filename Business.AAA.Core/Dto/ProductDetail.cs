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
        public ProductDetail()
        {
            ProductPrices = new List<ProductPricesDetail>();
        }

        public long ProductId { get; set; }

        [StringLength(32)]
        public string ProductCode { get; set; }

        [StringLength(32)]
        public string ProductDescription { get; set; }

        public decimal Quantity { get; set; }

        public List<ProductPricesDetail> ProductPrices { get; set; }

        public bool IsActive { get; set; }

        public long? CategoryId { get; set; }
    }

    public class ProductDetailRequest : BaseDetail
    {
        public ProductDetailRequest()
        {
            ProductPrices = new List<ProductPricesDetailRequest>();
        }

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

        public long? CategoryId { get; set; }

        public string Remarks { get; set; } = string.Empty;

        public List<ProductPricesDetailRequest> ProductPrices { get; set; }

        public decimal RetailerPrice { get; set; } = 0;
        public decimal ResellerPrice { get; set; } = 0;
        public decimal BigBuyerPrice { get; set; } = 0;

        public int PriceTypeId { get; set; }
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

        public long? CategoryId { get; set; }

        public bool? IsActive { get; set; }


    }

    public class ProductPricesDetail
    {
        public int PriceTypeId { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        

        public decimal Price { get; set; }
        public string PriceTypeName { get; set; }
    }

    public class ProductPricesDetailRequest
    {
        public int PriceTypeId { get; set; }
        public long ProductId { get; set; }
        public decimal Price { get; set; }
    }
}
