using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notepad.Repositories.Entities;

namespace Notepad.DataAccess
{
    public class NotepadContext : IdentityDbContext<User>
    {
        public NotepadContext(DbContextOptions<NotepadContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
