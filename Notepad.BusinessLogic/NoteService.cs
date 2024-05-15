using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Notepad.Common.DTOs;
using Notepad.Repositories;
using Notepad.Repositories.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notepad.BusinessLogic
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;
        private readonly FileManager _fileManager;
        private readonly IMapper _mapper;

        public NoteService(INoteRepository noteRepository, FileManager fileManager, IMapper mapper)
        {
            _noteRepository = noteRepository;
            _fileManager = fileManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NoteDTO>> GetAllNotesAsync(string name = "", int? categoryId = null)
        {
            var query = _noteRepository.GetAll();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(n => EF.Functions.Like(n.Title, $"%{name}%"));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryId.Value);
            }
            var notes = await query.ToListAsync();
            return _mapper.Map<IEnumerable<NoteDTO>>(notes);
        }

        public async Task<IEnumerable<NoteDTO>> GetNotesByUserIdAsync(string userId)
        {
            var notes = await _noteRepository.GetNotesByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<NoteDTO>>(notes);
        }

        public async Task<NoteDTO> GetNoteByIdAsync(int id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            return _mapper.Map<NoteDTO>(note);
        }

        public async Task CreateNoteAsync(NoteDTO noteDto)
        {
            var note = _mapper.Map<Note>(noteDto);
            if (noteDto.Image != null)
            {
                note.ImagePath = await _fileManager.SaveImageAsync(noteDto.Image);
            }
            await _noteRepository.AddAsync(note);
        }

        public async Task UpdateNoteAsync(NoteUpdateDTO noteUpdateDto)
        {
            var note = await _noteRepository.GetByIdAsync(noteUpdateDto.NoteId);
            _mapper.Map(noteUpdateDto, note);

            if (noteUpdateDto.Image != null)
            {
                note.ImagePath = await _fileManager.SaveImageAsync(noteUpdateDto.Image);
            }
            await _noteRepository.UpdateAsync(note);
        }

        public async Task DeleteNoteAsync(int id)
        {
            var note = await _noteRepository.GetByIdAsync(id);
            if (note != null)
            {
                await _noteRepository.DeleteAsync(note);
            }

        }
    }
}
