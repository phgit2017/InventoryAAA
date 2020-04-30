using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.InventoryAAA.Interface
{
    public interface IInventoryAAARepository<T>
    {
        IQueryable<T> SearchFor(Expression<Func<Task, bool>> predicate);
        IQueryable<T> GetAll();
        T Insert(T item);
        T Update(T item, Expression<Func<Task, bool>> predicate);
        T Update2(T item);
        bool Delete(Expression<Func<Task, bool>> predicate);
        IEnumerable<T> ExecuteQuery(string commandString, params object[] param);
        DataTable ExecuteSPReturnTable(string commandString, bool IsStoredProc, params object[] param);
        bool RemoveRange(Expression<Func<Task, bool>> predicate);
    }
}
