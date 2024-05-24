using Microsoft.EntityFrameworkCore;
using Notepad.Repositories;
using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.DataAccess
{
    public class NoteRepository : INoteRepository
    {
        private readonly NotepadContext _context;

        public NoteRepository(NotepadContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Note entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(Note entity)
        {
            _context.Notes.Remove(entity);
            _context.SaveChanges();
        }


        public IQueryable<Note> GetAll(string userId)
        {
            return _context.Notes.Where(n => n.UserId == userId).AsQueryable();
        }




        public async Task<Note> GetByIdAsync(int id, string userId) => await _context.Notes.FirstOrDefaultAsync(n => n.NoteId == id && n.UserId == userId);



        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(string userId)
        {
            return await _context.Notes.Where(n => n.UserId == userId).ToListAsync();
        }

        public async Task UpdateAsync(Note entity)
        {
            _context.Notes.Update(entity);
            await _context.SaveChangesAsync();
            ;
        }
    }
}
