using Microsoft.EntityFrameworkCore;
using MyBook.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace MyBook.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book_Author>()
                .HasOne(a => a.Book)
                .WithMany(b => b.Book_Authors)
                .HasForeignKey(c => c.BookId);

            modelBuilder.Entity<Book_Author>()
                .HasOne(a => a.Author)
                .WithMany(b => b.Book_Authors)
                .HasForeignKey(c => c.AuthorId);

            modelBuilder.Entity<Log>().HasKey(n => n.Id);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book_Author> Books_Authors { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<Log> Logs { get; set; }
    }
}
