using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.Repositories
{
    public interface INoteRepository
    {
        Task<Note> GetByIdAsync(int id);
        IQueryable<Note> GetAll(string userId);
        Task<IEnumerable<Note>> GetNotesByUserIdAsync(string userId);
        Task AddAsync(Note note);
        Task UpdateAsync(Note note);
        Task DeleteAsync(Note note);
    }
}
