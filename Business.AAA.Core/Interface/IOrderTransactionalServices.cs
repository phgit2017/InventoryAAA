using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Dto;

namespace Business.AAA.Core.Interface
{
    public interface IOrderTransactionalServices
    {
        long UpdateOrderTransaction(OrderTransactionRequest orderTransactionRequest,
            List<ProductDetailRequest> orderTransactionDetailRequest);
    }
}
