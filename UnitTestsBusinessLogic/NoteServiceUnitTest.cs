using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;
using Notepad.DataAccess;
using Notepad.Repositories.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsBusinessLogic
{
    public class NoteServiceUnitTest : IAsyncLifetime
    {
        private NotepadContext _context;
        private NoteService _noteService;

        private readonly Mock<IFileManager> _fileManagerMock = new Mock<IFileManager>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ILogger<NoteService>> _loggerMock = new Mock<ILogger<NoteService>>();

        private List<NoteDTO> _noteDTOs;
        private NoteUpdateDTO _noteUpdateDTO;
        private CreateNoteDTO _createNoteDTO;
        private NoteUpdateImageDTO _noteUpdateImageDTO;

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();

            _noteDTOs = new List<NoteDTO> {
                new NoteDTO { NoteId = 1, Title = "Note1", Content = "Content1", ImagePath = "path1.jpg", CategoryId = 1, UserId = "user1" },
                new NoteDTO { NoteId = 2, Title = "Note2", Content = "Content2", ImagePath = "path2.jpg", CategoryId = 2, UserId = "user2" }
            };

            _noteUpdateDTO = new NoteUpdateDTO
            {
                Title = "Updated Note",
                Content = "Updated Content",
                CategoryId = 1
            };

            _createNoteDTO = new CreateNoteDTO
            {
                Title = "New Note",
                Content = "New Content",
                CategoryId = 1,
                Image = null
            };

            _noteUpdateImageDTO = new NoteUpdateImageDTO
            {
                Image = new FormFile(new MemoryStream(), 0, 0, null, "test.jpg")
            };

            var noteRepositoryMock = new NoteRepository(_context);

            _mapperMock.Setup(m => m.Map<IEnumerable<NoteDTO>>(It.IsAny<IEnumerable<Note>>()))
                .Returns((IEnumerable<Note> source) =>
                {
                    return source.Select(s => new NoteDTO
                    {
                        NoteId = s.NoteId,
                        Title = s.Title,
                        Content = s.Content,
                        ImagePath = s.ImagePath, // Ensure image path is mapped
                        UserId = s.UserId,
                        CategoryId = s.CategoryId
                    });
                });

            _mapperMock.Setup(m => m.Map<NoteDTO>(It.IsAny<Note>()))
                .Returns((Note note) =>
                    note != null ? new NoteDTO
                    {
                        NoteId = note.NoteId,
                        Title = note.Title,
                        Content = note.Content,
                        ImagePath = note.ImagePath, // Ensure image path is mapped
                        UserId = note.UserId,
                        CategoryId = note.CategoryId
                    } : null);

            _mapperMock.Setup(m => m.Map<Note>(It.IsAny<CreateNoteDTO>())).Returns((CreateNoteDTO dto) =>
                new Note { Title = dto.Title, Content = dto.Content, CategoryId = dto.CategoryId, UserId = "user3", ImagePath = "default/path.jpg" });

            _mapperMock.Setup(m => m.Map(It.IsAny<NoteUpdateDTO>(), It.IsAny<Note>())).Callback<NoteUpdateDTO, Note>((dto, note) =>
            {
                note.Title = dto.Title;
                note.Content = dto.Content;
                note.CategoryId = dto.CategoryId.Value;
            });

            _fileManagerMock.Setup(fm => fm.SaveImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("path/to/image.jpg");
            _fileManagerMock.Setup(fm => fm.DeleteImageAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            _noteService = new NoteService(noteRepositoryMock, _fileManagerMock.Object, _mapperMock.Object, _loggerMock.Object);
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
        public async Task GetAllNotesAsync_ShouldReturnAllNotes()
        {
            // Act
            var result = await _noteService.GetAllNotesAsync("user1");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_noteDTOs[0].Title, result.First().Title);
            Assert.Equal(_noteDTOs[0].ImagePath, result.First().ImagePath); // Ensure ImagePath is checked
        }

        [Fact]
        public async Task GetNotesByUserIdAsync_ShouldReturnNotes_WhenUserHasNotes()
        {
            // Arrange
            var userId = "user1";
            var notesDTOs = _noteDTOs.FindAll(n => n.UserId == userId);

            // Act
            var result = await _noteService.GetNotesByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(notesDTOs.Count, result.Count());
            Assert.Equal(notesDTOs.Select(n => n.Title), result.Select(n => n.Title));
            Assert.Equal(notesDTOs.Select(n => n.ImagePath), result.Select(n => n.ImagePath)); // Ensure ImagePath is checked
        }

        [Fact]
        public async Task GetNoteByIdAsync_ShouldReturnNote_WhenNoteExists()
        {
            // Arrange
            var noteId = 1;

            // Act
            var result = await _noteService.GetNoteByIdAsync(noteId, "user1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(noteId, result.NoteId);
            Assert.Equal("path1.jpg", result.ImagePath); // Ensure ImagePath is checked
        }

        [Fact]
        public async Task GetNoteByIdAsync_ShouldReturnNull_WhenNoteDoesNotExist()
        {
            // Arrange
            var noteId = 3;

            // Act
            var result = await _noteService.GetNoteByIdAsync(noteId, "user1");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateNoteAsync_ShouldCreateNote()
        {
            // Arrange
            var userId = "user3";

            // Act
            var result = await _noteService.CreateNoteAsync(_createNoteDTO, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_createNoteDTO.Title, result.Title);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task UpdateNoteDetailsAsync_ShouldUpdateNote_WhenNoteExists()
        {
            // Arrange
            var noteId = 1;

            // Act
            await _noteService.UpdateNoteDetailsAsync(noteId, _noteUpdateDTO, "user1");

            // Assert
            var updatedNote = await _context.Notes.FindAsync(noteId);
            Assert.Equal(_noteUpdateDTO.Title, updatedNote.Title);
            Assert.Equal(_noteUpdateDTO.Content, updatedNote.Content);
        }

        [Fact]
        public async Task UpdateNoteImageAsync_ShouldUpdateNote_WhenNoteExists()
        {
            // Arrange
            var noteId = 1;
            var note = await _context.Notes.FindAsync(noteId);
            var existingImagePath = note.ImagePath;

            // Act
            await _noteService.UpdateNoteImageAsync(noteId, _noteUpdateImageDTO, "user1");

            // Assert
            var updatedNote = await _context.Notes.FindAsync(noteId);
            Assert.Equal("path/to/image.jpg", updatedNote.ImagePath);
            _fileManagerMock.Verify(fm => fm.DeleteImageAsync(existingImagePath), Times.Once);
            _fileManagerMock.Verify(fm => fm.SaveImageAsync(_noteUpdateImageDTO.Image), Times.Once);
        }

        [Fact]
        public async Task DeleteNoteAsync_ShouldDeleteNote_WhenNoteExists()
        {
            // Arrange
            var noteId = 1;

            // Act
            await _noteService.DeleteNoteAsync(noteId, "user1");

            // Assert
            var deletedNote = await _context.Notes.FindAsync(noteId);
            Assert.Null(deletedNote);
        }
    }
}
