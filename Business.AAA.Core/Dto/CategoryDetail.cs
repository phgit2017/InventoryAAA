using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class CategoryDetail : BaseDetail
    {
        public long CategoryId { get; set; }
        
        public string CategoryName { get; set; }

        public bool IsActive { get; set; }
    }

    public class CategoryDetailRequest : BaseDetail
    {
        public long CategoryId { get; set; }

        [Required]
        [StringLength(128)]
        public string CategoryName { get; set; }

        public bool IsActive { get; set; }
        
    }
}
