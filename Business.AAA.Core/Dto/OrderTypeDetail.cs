using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class OrderTypeDetail
    {
        public int OrderTypeId { get; set; }

        [Required]
        [StringLength(16)]
        public string OrderTypeName { get; set; }
    }

    public class OrderTypeActionDetail
    {
        public int OrderId { get; set; }

        public string OrderActionName { get; set; }
    }
}
