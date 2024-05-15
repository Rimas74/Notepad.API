using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notepad.Repositories.Entities;

namespace Notepad.DataAccess
{
    public class NotepadContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Note> Notes { get; set; }

        public NotepadContext(DbContextOptions<NotepadContext> options) : base(options) { }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Note>().HasKey(n => n.NoteId);

            modelBuilder.Entity<Note>()
        .HasOne(n => n.User)
        .WithMany(u => u.Notes)
        .HasForeignKey(n => n.UserId)
        .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Note>()
                .HasOne(n => n.Category)
                .WithMany(c => c.Notes)
                .HasForeignKey(n => n.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
        .HasOne(c => c.User)
        .WithMany(u => u.Categories)
        .HasForeignKey(c => c.UserId);
        }
    }
}
