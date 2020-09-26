using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Dto
{
    public class CustomerDetail : BaseDetail
    {
        public long CustomerId { get; set; }

        public string CustomerCode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullAddress { get; set; }

        public int CustomerStatusId { get; set; }

        public string CustomerStatusName { get; set; }

        public string FullName {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }

    public class CustomerStatusDetail
    {
        public int CustomerStatusId { get; set; }

        public string CustomerStatusName { get; set; }
    }

    public class CustomerDetailRequest : BaseDetail
    {
        public long CustomerId { get; set; }

        [StringLength(32)]
        public string CustomerCode { get; set; }

        [StringLength(128)]
        public string FirstName { get; set; }

        [StringLength(128)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string FullAddress { get; set; }

        public int CustomerStatusId { get; set; }
    }
}
