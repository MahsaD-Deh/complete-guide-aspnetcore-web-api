using MyBook.Data.Models;
using MyBook.Data.ViewModels;

namespace MyBook.Data.Services
{
    public class AuthorsService
    {
        private AppDbContext _context;

        public AuthorsService(AppDbContext context)
        {
            _context = context;
        }

        #region AddAuthor()
        public void AddAuthor(AuthorVM author)
        {
            var _author = new Author()
            {
               FullName = author.FullName
            };

            _context.Authors.Add(_author);
            _context.SaveChanges();
        }
        #endregion

        public AuthorWithBookVM GetAuthorWithBooks(int authorId)
        {
            var _author = _context.Authors.Where(n => n.Id == authorId).Select(n => new AuthorWithBookVM()
            {
                FullName = n.FullName,
                BookTitles = n.Book_Authors.Select(n => n.Book.Title).ToList()
            }).FirstOrDefault();

            return _author;
        }
    }
}
