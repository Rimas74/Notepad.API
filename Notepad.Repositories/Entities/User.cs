using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.Repositories.Entities
{
    public class User : IdentityUser
    {
        public ICollection<Note> Notes { get; set; }
        public ICollection<Category> Categories { get; set; }

    }
}
