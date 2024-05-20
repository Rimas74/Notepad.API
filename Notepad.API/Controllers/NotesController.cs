using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        //[Authorize]
        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetNotes([FromQuery] string? name, [FromQuery] int? categoryId)
        {

            var filteredNotes = await _noteService.GetAllNotesAsync(name, categoryId);
            return Ok(filteredNotes);


        }

        [HttpGet("{id}")]
        //[Authorize]
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
        //[Authorize]
        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetNotesByUserId(string userId)
        {
            var notes = await _noteService.GetNotesByUserIdAsync(userId);
            return Ok(notes);
        }

        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<NoteDTO>> CreateNote([FromForm] CreateNoteDTO createNoteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var createdNote = await _noteService.CreateNoteAsync(createNoteDto, userId);
            if (createdNote == null)
            {
                return BadRequest("Failed to create the note.");
            }
            return CreatedAtAction(nameof(GetNoteById), new { id = createdNote.NoteId }, createdNote); //return Ok(createdNote);
        }

        [HttpPut("{id}/details")]
        [Authorize]
        public async Task<IActionResult> UpdateNoteDetails(int id, [FromBody] NoteUpdateDTO noteUpdateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = await _noteService.GetNoteByIdAsync(id);

            if (note == null || note.UserId != userId)
            {
                return NotFound();
            }

            await _noteService.UpdateNoteDetailsAsync(id, noteUpdateDto);
            return NoContent();
        }

        [HttpPut("{id}/image")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateNoteImage(int id, [FromForm] NoteUpdateImageDTO noteUpdateImageDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = await _noteService.GetNoteByIdAsync(id);

            if (note == null || note.UserId != userId)
            {
                return NotFound();
            }

            await _noteService.UpdateNoteImageAsync(id, noteUpdateImageDto);
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize]
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
