using Notepad.DataAccess;
using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.BusinessLogic
{
    public interface ICategoryService : IService<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesByUserIdAsync(string userId);
    }
}
