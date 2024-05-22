using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Notepad.BusinessLogic;
using Notepad.Common.DTOs;
using Notepad.DataAccess;
using Notepad.Repositories;
using Notepad.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task InitializeAsync()
        {
            _context = await GetDatabaseContext();


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
        }

        public Task DisposeAsync()
        {
            _context?.Dispose();
            return Task.CompletedTask;
        }


    }
}
