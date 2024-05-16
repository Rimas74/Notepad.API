﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;
using System.Security.Claims;

namespace Notepad.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetNotes([FromQuery] string? name, [FromQuery] int? categoryId)
        {

            var filteredNotes = await _noteService.GetAllNotesAsync(name, categoryId);
            return Ok(filteredNotes);


        }
        [HttpGet("{id}")]
        public async Task<ActionResult<NoteDTO>> GetNoteById(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            return Ok(note);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetNotesByUserId(string userId)
        {
            var notes = await _noteService.GetNotesByUserIdAsync(userId);
            return Ok(notes);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<NoteDTO>> CreateNote([FromBody] CreateNoteDTO createNoteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var createdNote = await _noteService.CreateNoteAsync(createNoteDto, userId);
            if (createdNote == null)
            {
                return BadRequest("Failed to create the note.");
            }
            return CreatedAtAction(nameof(GetNoteById), new { id = createdNote.NoteId }, createdNote);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] NoteUpdateDTO noteUpdateDto)
        {
            await _noteService.UpdateNoteAsync(id, noteUpdateDto);

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            await _noteService.DeleteNoteAsync(id);
            return NoContent();
        }

    }

}
