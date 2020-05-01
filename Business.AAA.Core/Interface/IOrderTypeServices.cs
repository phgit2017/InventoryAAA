using Business.AAA.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AAA.Core.Interface
{
    public interface IOrderTypeServices
    {
        IQueryable<OrderTypeDetail> GetAll();
    }
}
