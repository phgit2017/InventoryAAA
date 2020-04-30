using DataAccess.Entities.Context.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities.Context
{
    public class AAAInventoryEntities : System.Data.Entity.DbContext, IAAAInventoryEntities
    {
        public AAAInventoryEntities()
            : base("name=")
        { }

    }
}
