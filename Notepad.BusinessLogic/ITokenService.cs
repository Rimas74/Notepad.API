using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.BusinessLogic
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
    }
}
