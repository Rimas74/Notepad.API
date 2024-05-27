using Microsoft.EntityFrameworkCore;
using Notepad.DataAccess;
using Notepad.Repositories.Entities;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTestsDataAccess
{
    public class NoteRepositoryTests : IAsyncLifetime
    {
        private NotepadContext _context;
        private NoteRepository _repository;

        public Task DisposeAsync()
        {
            _context?.Dispose();
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();
            _repository = new NoteRepository(_context);
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
                context.Notes.AddRange(
                    new Note { NoteId = 1, Title = "Note1", Content = "Content1", UserId = "user1", CategoryId = 1, ImagePath = "path1.jpg" },
                    new Note { NoteId = 2, Title = "Note2", Content = "Content2", UserId = "user2", CategoryId = 2, ImagePath = "path2.jpg" }
                );
                await context.SaveChangesAsync();
            }
            return context;
        }

        [Fact]
        public async Task AddAsync_ShouldAddNote()
        {
            var note = new Note { Title = "NewNote", Content = "NewContent", UserId = "user3", CategoryId = 1, ImagePath = "newpath.jpg" };
            await _repository.AddAsync(note);
            var notes = await _context.Notes.ToListAsync();

            Assert.Contains(note, notes);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveNote()
        {
            var note = await _context.Notes.FirstAsync();

            await _repository.DeleteAsync(note);
            var notes = await _context.Notes.ToListAsync();

            Assert.DoesNotContain(note, notes);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllNotes()
        {
            var notes = await _repository.GetAll("user1").ToListAsync();

            Assert.Single(notes);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNoteById()
        {
            var note = await _repository.GetByIdAsync(1, "user1");

            Assert.NotNull(note);
            Assert.Equal(1, note.NoteId);
        }

        [Fact]
        public async Task GetNotesByUserIdAsync_ShouldReturnNotesByUserId()
        {
            var notes = await _repository.GetNotesByUserIdAsync("user1");

            Assert.Single(notes);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateNote()
        {
            var note = await _context.Notes.FirstAsync();
            note.Title = "UpdatedNote";

            await _repository.UpdateAsync(note);
            var updatedNote = await _context.Notes.FindAsync(note.NoteId);

            Assert.Equal("UpdatedNote", updatedNote.Title);
        }
    }
}
