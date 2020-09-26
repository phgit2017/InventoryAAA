using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.AAA.Core.Dto;

namespace Business.AAA.Core.Interface
{
    public interface ICategoryServices
    {
        IQueryable<CategoryDetail> GetAll();
        long SaveDetail(CategoryDetailRequest request);
        bool UpdateDetail(CategoryDetailRequest request);
    }
}
