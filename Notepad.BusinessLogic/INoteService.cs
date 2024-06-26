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
        Task<IEnumerable<NoteDTO>> GetAllNotesAsync(string userId, string name, int? categoryId);
        Task<IEnumerable<NoteDTO>> GetNotesByUserIdAsync(string userId);
        Task<NoteDTO> GetNoteByIdAsync(int id, string userId);
        Task<NoteDTO> CreateNoteAsync(CreateNoteDTO noteDto, string userId);

        Task UpdateNoteDetailsAsync(int noteId, NoteUpdateDTO noteUpdateDto, string userId);
        Task UpdateNoteImageAsync(int noteId, NoteUpdateImageDTO noteUpdateImageDto, string userId);
        Task DeleteNoteAsync(int id, string userId);
    }
}
