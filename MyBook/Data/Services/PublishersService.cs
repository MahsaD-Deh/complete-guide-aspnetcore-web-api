using MyBook.Data.Models;
using MyBook.Data.ViewModels;
using MyBook.Exceptions;
using System.Text.RegularExpressions;
using static Azure.Core.HttpHeader;

namespace MyBook.Data.Services
{
    public class PublishersService
    {
        private AppDbContext _context;

        public PublishersService(AppDbContext context)
        {
            _context = context;
        }

        #region AddPublisher()
        public Publisher AddPublisher(PublisherVM publisher)
        {
            if (StringStartsWithNumber(publisher.Name))
            {
                throw new PublisherNameException("Name starts with number", publisher.Name);
            }
            var _publisher = new Publisher()
            {
                Name = publisher.Name
            };

            _context.Publishers.Add(_publisher);
            _context.SaveChanges();

            return _publisher;
        }
        #endregion


        #region GetPublisherData()
        public PublisherWithBooksAndAuthorsVM GetPublisherData(int publisherId)
        {
            var _publisherData = _context.Publishers.Where(n => n.Id == publisherId)
                .Select(b => new PublisherWithBooksAndAuthorsVM()
                {
                    Name = b.Name,
                    BookAuthors = b.Books.Select(m => new BookAuthorVM()
                    {
                        BookName = m.Title,
                        BookAuthors = m.Book_Authors.Select(k => k.Author.FullName).ToList()
                    }).ToList()
                }).FirstOrDefault();

            return _publisherData;
        }
        #endregion


        #region DeletePublisher()
        public void DeletePublisher(int id)
        {
            var _publisher = _context.Publishers.FirstOrDefault(n => n.Id == id);

            if (_publisher != null)
            {
                _context.Publishers.Remove(_publisher);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($"The publisher with id: {id} does not exist");
            }
        }
        #endregion


        public Publisher GetPublisherById(int id) => _context.Publishers.FirstOrDefault(n => n.Id == id);


        private bool StringStartsWithNumber(string name) => Regex.IsMatch(name, @"^\d");

    }
}
