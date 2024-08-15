using Microsoft.EntityFrameworkCore;
using MyBook.Data;
using MyBook.Data.Models;
using MyBook.Data.Services;

namespace my_books_tests
{
    public class PublisherServiceTest
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName : "MyBooksDBTest")
            .Options;


        AppDbContext context;
        PublishersService publishersService;
        
        [OneTimeSetUp]
        public void Setup()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();
            publishersService = new PublishersService(context);
        }
        
        [Test]
        public void GetAllPublishers_Test()
        {
            var result = publishersService.GetAllPublishers("","");
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetPublisherById_Test()
        {
            var result = publishersService.GetPublisherById(1);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Pub1"));
        }


        [OneTimeTearDown]
        public void CòeanUp()
        {
            context.Database.EnsureDeleted(); 
        }

        private void SeedDatabase()
        {
            var publishers = new List<Publisher>
            {
                new Publisher()
                {
                    Id =1,
                    Name = "Pub1"
                },
                new Publisher()
                {
                    Id =2,
                    Name = "Pub2"
                },
                new Publisher()
                {
                    Id =3,
                    Name = "Pub3"
                },
            };

            context.Publishers.AddRange(publishers);

            var authors = new List<Author>
            {
                new Author()
                {
                    Id =1,
                    FullName = "Author1"
                },
                new Author()
                {
                    Id =2,
                    FullName = "Author2"
                },
                new Author()
                {
                    Id =3,
                    FullName = "Author3"
                },
            };

            context.Authors.AddRange(authors);

            var book_Author = new List<Book_Author>
            {
                new Book_Author()
                {
                    Id =1,
                    BookId = 1,
                    AuthorId = 1
                },
                new Book_Author()
                {
                    Id =2,
                    BookId = 1,
                    AuthorId = 2
                },
                new Book_Author()
                {
                    Id =3,
                    BookId = 2,
                    AuthorId = 2
                },
            };

            context.Books_Authors.AddRange(book_Author);

            var book = new List<Book>
            {
                new Book()
                {
                    Id =1,
                    Title = "Book 1",
                    Description = "Desc1",
                    IsRead = false,
                    Genre ="Comedy",
                    CoverUrl = "http1",
                    DateAdded = DateTime.Now.AddDays(-10),
                    PublisherId = 1
                },
                new Book()
                {
                    Id =2,
                    Title = "Book 2",
                    Description = "Desc2",
                    IsRead = false,
                    Genre ="Horror",
                    CoverUrl = "http2",
                    DateAdded = DateTime.Now.AddDays(-10),
                    PublisherId = 1
                },
            };

            context.Books.AddRange(book);

            context.SaveChanges();
        }

    }
}