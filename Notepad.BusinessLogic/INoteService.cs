﻿using Notepad.DataAccess;
using Notepad.Repositories.Entities;
using Notepad.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Notepad.BusinessLogic
{
    public interface INoteService
    {
        Task<IEnumerable<NoteDTO>> GetAllNotesAsync(string name, int? categoryId);
        Task<IEnumerable<NoteDTO>> GetNotesByUserIdAsync(string userId);
        Task<NoteDTO> GetNoteByIdAsync(int id);
        Task CreateNoteAsync(NoteDTO noteDto);
        Task UpdateNoteAsync(int id, NoteUpdateDTO noteUpdateDto);
        Task DeleteNoteAsync(int id);
    }
}