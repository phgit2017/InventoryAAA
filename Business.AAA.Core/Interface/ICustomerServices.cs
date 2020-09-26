using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Dto;

namespace Business.AAA.Core.Interface
{
    public interface ICustomerServices
    {
        IQueryable<CustomerDetail> GetAll();
        IQueryable<CustomerStatusDetail> GetAllCustomerStatus();
        long SaveDetail(CustomerDetailRequest request);
        bool UpdateDetail(CustomerDetailRequest request);
    }
}
