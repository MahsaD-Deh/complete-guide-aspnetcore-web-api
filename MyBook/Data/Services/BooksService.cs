using MyBook.Data.Models;
using MyBook.Data.ViewModels;

namespace MyBook.Data.Services
{
    public class BooksService
    {
        private AppDbContext _context;

        public BooksService(AppDbContext context)
        {
            _context = context;
        }

        #region AddBook()
        public void AddBook(BookVM book)
        {
            var _book = new Book()
            {
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.IsRead ? book.DateRead : null,
                Rate = book.IsRead ? book.Rate : null,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                DateAdded = DateTime.Now,
                PublisherId = book.PublisherId,
            };

            _context.Books.Add(_book);
            _context.SaveChanges();

            foreach (var id in book.AuthorsIds)
            {
                var _bookAuthor = new Book_Author()
                {
                    BookId = _book.Id,
                    AuthorId = id
                };

                _context.Books_Authors.Add(_bookAuthor);
                _context.SaveChanges();
            }
        }
        #endregion


        #region GetAllBooks()

        // When we have just one line in body of method we can write in this way :
        public List<Book> GetAllBooks() => _context.Books.ToList();
        #endregion


        #region GetBookById(int bookId)
        public BookWithAuthorsVM GetBookById(int bookId)
        {
            var _bookWithAuthors = _context.Books.Where(n => n.Id == bookId).Select(book => new BookWithAuthorsVM()
            {
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.IsRead ? book.DateRead : null,
                Rate = book.IsRead ? book.Rate : null,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                PublisherName = book.Publisher.Name,
                AuthorNames = book.Book_Authors.Select(n => n.Author.FullName).ToList()
            }).FirstOrDefault();

            return _bookWithAuthors;
        }

        #endregion


        #region UpdateBookById(int bookId, BookVM book)
        public Book UpdateBookById(int bookId, BookVM book)
        {
            var _book = _context.Books.FirstOrDefault(n => n.Id == bookId);
            if (_book != null)
            {
                _book.Title = book.Title;
                _book.Description = book.Description;
                _book.IsRead = book.IsRead;
                _book.DateRead = book.IsRead ? book.DateRead : null;
                _book.Rate = book.IsRead ? book.Rate : null;
                _book.Genre = book.Genre;
                _book.CoverUrl = book.CoverUrl;

                _context.SaveChanges();
            }
            return _book;
        }
        #endregion


        #region DeleteBookById(int bookId)
        public bool DeleteBookById(int bookId)
        {
            var _book = _context.Books.FirstOrDefault(n => n.Id == bookId);
            if (_book == null)
            {
                return false;
            }
            _context.Books.Remove(_book);
            _context.SaveChanges();
            return true;
        }
        #endregion
    }
}
