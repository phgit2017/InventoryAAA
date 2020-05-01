using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using dbentities = DataAccess.Database.InventoryAAA;
using DataAccess.Database.InventoryAAA;
using DataAccess.Repository.InventoryAAA.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Business.AAA.Core
{
    public partial class OrderTypeServices
    {
        IInventoryAAARepository<dbentities.OrderType> _orderTypes;

        private dbentities.OrderType orderTypes;

        public OrderTypeServices(IInventoryAAARepository<dbentities.OrderType> orderTypes)
        {
            this._orderTypes = orderTypes;

            this.orderTypes = new dbentities.OrderType();
        }
    }

    public partial class OrderTypeServices : IOrderTypeServices
    {
        public IQueryable<OrderTypeDetail> GetAll()
        {
            var result = from det in this._orderTypes.GetAll()
                         select new OrderTypeDetail()
                         {
                             OrderTypeId = det.OrderTypeID,
                             OrderTypeName = det.OrderTypeName
                         };

            return result;
        }
    }
}
