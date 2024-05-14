﻿using Microsoft.EntityFrameworkCore;
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



        public async Task DeleteAsync(int id)
        {
            var noteToDelete = await GetByIdAsync(id);
            if (noteToDelete != null)
            {
                _context.Notes.Remove(noteToDelete);
                _context.SaveChanges();
            }
        }

        public IQueryable<Note> GetAll()
        {
            return _context.Notes.AsQueryable();
        }




        public async Task<Note> GetByIdAsync(int id) => await _context.Notes.FindAsync(id);



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