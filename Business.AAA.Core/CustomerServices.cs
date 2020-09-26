using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Extensions;
using Business.AAA.Core.Dto;
using Business.AAA.Core.Interface;
using dbentities = DataAccess.Database.InventoryAAA;
using DataAccess.Database.InventoryAAA;
using DataAccess.Repository.InventoryAAA.Interface;
using Infrastructure.Utilities;
using System.Data.SqlClient;
using System.Data;

namespace Business.AAA.Core
{
    public partial class CustomerServices
    {
        IInventoryAAARepository<dbentities.Customer> _customerServices;
        IInventoryAAARepository<dbentities.CustomerStatus> _customerStatusServices;

        private dbentities.Customer customers;
        private dbentities.CustomerStatus customerStatus;

        public CustomerServices(
            IInventoryAAARepository<dbentities.Customer> customerServices,
            IInventoryAAARepository<dbentities.CustomerStatus> customerStatusServices)
        {
            this._customerServices = customerServices;
            this._customerStatusServices = customerStatusServices;

            this.customers = new dbentities.Customer();
            this.customerStatus = new dbentities.CustomerStatus();

        }
    }

    public partial class CustomerServices : ICustomerServices
    {
        public IQueryable<CustomerDetail> GetAll()
        {
            var result = from det in this._customerServices.GetAll()
                         select new CustomerDetail()
                         {
                             CustomerId = det.CustomerID,
                             CustomerCode = det.CustomerCode,
                             CustomerStatusId = det.CustomerStatusID,
                             CustomerStatusName = det.CustomerStatus.CustomerStatusName,
                             FirstName = det.FirstName,
                             LastName = det.LastName,
                             FullAddress = det.FullAddress,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime
                         };

            return result;
        }

        public IQueryable<CustomerStatusDetail> GetAllCustomerStatus()
        {
            var result = from det in this._customerStatusServices.GetAll()
                         select new CustomerStatusDetail()
                         {
                             CustomerStatusId = det.CustomerStatusID,
                             CustomerStatusName = det.CustomerStatusName
                         };

            return result;
        }

        public long SaveDetail(CustomerDetailRequest request)
        {
            request.CustomerId = 0;

            
            var customerNameResult = GetAll().Where(u => u.FirstName == request.FirstName
                                                           && u.CustomerStatusId == LookupKey.CustomerStatus.ActiveId
                                                           && u.LastName == request.LastName).FirstOrDefault();

            #region Validate same firstname and lastname
            if (!customerNameResult.IsNull())
            {
                return -100;
            }
            #endregion

            this.customers = request.DtoToEntity();
            var item = this._customerServices.Insert(this.customers);
            if (item == null)
            {
                return 0;
            }

            return item.CustomerID;
        }

        public bool UpdateDetail(CustomerDetailRequest request)
        {
            this.customers = request.DtoToEntity();
            var item = _customerServices.Update2(this.customers);
            if (item == null)
            {
                return false;
            }

            return true;
        }
    }
}
