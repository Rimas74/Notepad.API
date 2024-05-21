﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;
        private readonly ILogger<NoteService> _logger;


        public NoteService(INoteRepository noteRepository, IFileManager fileManager, IMapper mapper, ILogger<NoteService> logger)
        {
            _noteRepository = noteRepository;
            _fileManager = fileManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<NoteDTO>> GetAllNotesAsync(string? name = "", int? categoryId = null)
        {
            _logger.LogInformation("Getting all notes with filter name={name} and categoryId={categoryId}", name, categoryId);
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
            _logger.LogInformation("Getting notes for userId={userId}", userId);
            var notes = await _noteRepository.GetNotesByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<NoteDTO>>(notes);
        }

        public async Task<NoteDTO> GetNoteByIdAsync(int id)
        {
            _logger.LogInformation("Getting note by id={id}", id);
            var note = await _noteRepository.GetByIdAsync(id);
            return _mapper.Map<NoteDTO>(note);
        }

        public async Task<NoteDTO> CreateNoteAsync(CreateNoteDTO createNoteDto, string userId)
        {
            _logger.LogInformation("Creating note for userId={userId}", userId);
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
            _logger.LogInformation("Updating note id={noteId}", noteId);
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
            _logger.LogInformation("Deleting note id={id}", id);
            var note = await _noteRepository.GetByIdAsync(id);
            if (note != null)
            {
                await _noteRepository.DeleteAsync(note);
            }

        }



        public async Task UpdateNoteImageAsync(int noteId, NoteUpdateImageDTO noteUpdateImageDto)
        {
            _logger.LogInformation("Updating image of note id={noteId}", noteId);
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
