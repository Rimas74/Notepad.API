using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.DataAccess
{
    public interface IService<T> where T : class
    {
        Task<T> GetAsyncById(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(int id);
        Task DeleteAsync(T entity);
    }
}
