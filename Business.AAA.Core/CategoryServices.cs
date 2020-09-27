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
    public partial class CategoryServices
    {
        IInventoryAAARepository<dbentities.Categories> _categoryServices;

        private dbentities.Categories categories;

        public CategoryServices(
            IInventoryAAARepository<dbentities.Categories> categoryServices)
        {
            this._categoryServices = categoryServices;

            this.categories = new dbentities.Categories();

        }
    }

    public partial class CategoryServices : ICategoryServices
    {
        public long SaveDetail(CategoryDetailRequest request)
        {
            request.CategoryId = 0;
            var categoryNameResult = GetAll().Where(u => u.CategoryName == request.CategoryName
                                                           && u.IsActive).FirstOrDefault();

            #region Validate same firstname and lastname
            if (!categoryNameResult.IsNull())
            {
                return -100;
            }
            #endregion

            this.categories = request.DtoToEntity();
            var item = this._categoryServices.Insert(this.categories);
            if (item == null)
            {
                return 0;
            }

            return item.CategoryID;
        }

        public bool UpdateDetail(CategoryDetailRequest request)
        {
            this.categories = request.DtoToEntity();
            var item = _categoryServices.Update2(this.categories);
            if (item == null)
            {
                return false;
            }

            return true;
        }

        public IQueryable<CategoryDetail> GetAll()
        {
            var result = from det in this._categoryServices.GetAll()
                         select new CategoryDetail()
                         {
                             CategoryId = det.CategoryID,
                             CategoryName = det.CategoryName,
                             IsActive = det.IsActive,
                             CreatedBy = det.CreatedBy,
                             CreatedTime = det.CreatedTime,
                             ModifiedBy = det.ModifiedBy,
                             ModifiedTime = det.ModifiedTime
                         };

            return result;
        }
    }
}
