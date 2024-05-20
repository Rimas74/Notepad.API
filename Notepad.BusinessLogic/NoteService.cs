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

        public async Task<IEnumerable<NoteDTO>> GetAllNotesAsync(string? name = "", int? categoryId = null)
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

        public async Task<NoteDTO> CreateNoteAsync(CreateNoteDTO createNoteDto, string userId)
        {
            var note = _mapper.Map<Note>(createNoteDto);
            note.UserId = userId;

            if (createNoteDto.Image != null)
            {
                note.ImagePath = await _fileManager.SaveImageAsync(createNoteDto.Image);
            }

            await _noteRepository.AddAsync(note);

            return _mapper.Map<NoteDTO>(note);
        }

        public async Task UpdateNoteDetailsAsync(int noteId, NoteUpdateDTO noteUpdateDto)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);
            if (note == null)
            {
                throw new KeyNotFoundException("Note not found");
            }
            _mapper.Map(noteUpdateDto, note);
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



        public async Task UpdateNoteImageAsync(int noteId, NoteUpdateImageDTO noteUpdateImageDto)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);
            if (note == null)
            {
                throw new KeyNotFoundException("Note not found");
            }
            if (noteUpdateImageDto.Image != null)
            {
                note.ImagePath = await _fileManager.SaveImageAsync(noteUpdateImageDto.Image);
            }
            await _noteRepository.UpdateAsync(note);
        }
    }
}
