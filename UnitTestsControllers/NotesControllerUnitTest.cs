using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Notepad.API.Controllers;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;
using Notepad.DataAccess;
using Notepad.Repositories.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsControllers
{
    public class NotesControllerUnitTest : IAsyncLifetime
    {
        private NotepadContext _context;
        private NotesController _notesController;

        private readonly Mock<IFileManager> _fileManagerMock = new Mock<IFileManager>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ILogger<NoteService>> _loggerMock = new Mock<ILogger<NoteService>>();
        private INoteService _noteService;

        private List<NoteDTO> _noteDTOs;
        private CreateNoteDTO _createNoteDTO;
        private NoteUpdateDTO _noteUpdateDTO;
        private NoteUpdateImageDTO _noteUpdateImageDTO;

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();

            _noteDTOs = new List<NoteDTO> {
                new NoteDTO { NoteId = 1, Title = "Note1", Content = "Content1", CategoryId = 1, UserId = "user1" },
                new NoteDTO { NoteId = 2, Title = "Note2", Content = "Content2", CategoryId = 2, UserId = "user2" }
            };

            _createNoteDTO = new CreateNoteDTO
            {
                Title = "New Note",
                Content = "New Content",
                CategoryId = 1,
                Image = null
            };

            _noteUpdateDTO = new NoteUpdateDTO
            {
                Title = "Updated Note",
                Content = "Updated Content",
                CategoryId = 1
            };

            _noteUpdateImageDTO = new NoteUpdateImageDTO
            {
                Image = new FormFile(new MemoryStream(), 0, 0, null, "test.jpg")
            };

            var noteRepository = new NoteRepository(_context);

            _mapperMock.Setup(m => m.Map<IEnumerable<NoteDTO>>(It.IsAny<IEnumerable<Note>>()))
                .Returns((IEnumerable<Note> source) => source.Select(s => new NoteDTO { NoteId = s.NoteId, Title = s.Title, Content = s.Content, UserId = s.UserId, CategoryId = s.CategoryId }));

            _mapperMock.Setup(m => m.Map<NoteDTO>(It.IsAny<Note>())).Returns((Note note) =>
                note != null ? new NoteDTO { NoteId = note.NoteId, Title = note.Title, Content = note.Content, UserId = note.UserId, CategoryId = note.CategoryId } : null);

            _mapperMock.Setup(m => m.Map<Note>(It.IsAny<CreateNoteDTO>())).Returns((CreateNoteDTO dto) =>
                new Note { Title = dto.Title, Content = dto.Content, CategoryId = dto.CategoryId, UserId = "user1", ImagePath = "default/path.jpg" });

            _mapperMock.Setup(m => m.Map(It.IsAny<NoteUpdateDTO>(), It.IsAny<Note>())).Callback<NoteUpdateDTO, Note>((dto, note) =>
            {
                note.Title = dto.Title;
                note.Content = dto.Content;
                note.CategoryId = dto.CategoryId.Value;
            });

            _fileManagerMock.Setup(fm => fm.SaveImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("path/to/image.jpg");

            _noteService = new NoteService(noteRepository, _fileManagerMock.Object, _mapperMock.Object, _loggerMock.Object);

            _notesController = new NotesController(_noteService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "user1")
                        }, "mock"))
                    }
                }
            };
        }

        private async Task<NotepadContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<NotepadContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new NotepadContext(options);
            context.Database.EnsureCreated();

            if (!context.Notes.Any())
            {
                context.Notes.AddRange
                (
                    new Note { NoteId = 1, Title = "Note1", Content = "Content1", ImagePath = "path1.jpg", CategoryId = 1, UserId = "user1" },
                    new Note { NoteId = 2, Title = "Note2", Content = "Content2", ImagePath = "path2.jpg", CategoryId = 2, UserId = "user2" }
                );
                await context.SaveChangesAsync();
            }
            return context;
        }

        public Task DisposeAsync()
        {
            _context?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetAllNotesAsync_ShouldReturnOk_WhenNotesExist()
        {
            // Act
            var result = await _notesController.GetNotes(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<NoteDTO>>(okResult.Value).ToList();
            Assert.Single(returnValue);
            Assert.Equal("user1", returnValue[0].UserId);
        }

        [Fact]
        public async Task GetNoteById_ShouldReturnOk_WhenNoteExists()
        {
            // Arrange
            var noteId = 1;

            // Act
            var result = await _notesController.GetNoteById(noteId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<NoteDTO>(okResult.Value);
            Assert.Equal(noteId, returnValue.NoteId);
        }

        [Fact]
        public async Task GetNoteById_ShouldReturnNotFound_WhenNoteDoesNotExist()
        {
            // Arrange
            var noteId = 3;

            // Act
            var result = await _notesController.GetNoteById(noteId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateNote_ShouldReturnCreatedNote_WhenNoteIsCreated()
        {
            // Act
            var result = await _notesController.CreateNote(_createNoteDTO);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<NoteDTO>(createdAtActionResult.Value);
            Assert.Equal(_createNoteDTO.Title, returnValue.Title);
        }

        [Fact]
        public async Task UpdateNoteDetails_ShouldReturnNoContent_WhenNoteIsUpdated()
        {
            // Arrange
            var noteId = 1;

            // Act
            var result = await _notesController.UpdateNoteDetails(noteId, _noteUpdateDTO);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedNote = await _context.Notes.FindAsync(noteId);
            Assert.Equal(_noteUpdateDTO.Title, updatedNote.Title);
            Assert.Equal(_noteUpdateDTO.Content, updatedNote.Content);
        }

        [Fact]
        public async Task UpdateNoteImage_ShouldReturnNoContent_WhenNoteImageIsUpdated()
        {
            // Arrange
            var noteId = 1;

            // Act
            var result = await _notesController.UpdateNoteImage(noteId, _noteUpdateImageDTO);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedNote = await _context.Notes.FindAsync(noteId);
            Assert.Equal("path/to/image.jpg", updatedNote.ImagePath);
        }

        [Fact]
        public async Task DeleteNote_ShouldReturnNoContent_WhenNoteIsDeleted()
        {
            // Arrange
            var noteId = 1;

            // Act
            var result = await _notesController.DeleteNote(noteId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var deletedNote = await _context.Notes.FindAsync(noteId);
            Assert.Null(deletedNote);
        }
    }
}
