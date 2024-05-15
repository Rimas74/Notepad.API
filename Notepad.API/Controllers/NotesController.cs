using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;

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
        public async Task<ActionResult<IEnumerable<NoteDTO>>> GetNotes([FromQuery] string name, [FromQuery] int? categoryId)
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
        public async Task<ActionResult<NoteDTO>> CreateNote([FromBody] NoteDTO noteDto)
        {
            await _noteService.CreateNoteAsync(noteDto);
            return CreatedAtAction(nameof(GetNoteById), new { id = noteDto.NoteId }, noteDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] NoteUpdateDTO noteUpdateDto)
        {
            if (id != noteUpdateDto.NoteId)
            {
                return BadRequest("Note ID mismatch");
            }

            await _noteService.UpdateNoteAsync(noteUpdateDto);
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
