using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MyBook.Controllers;
using MyBook.Data;
using MyBook.Data.Models;
using MyBook.Data.Services;

namespace my_books_tests
{
    public class PublishersControllerTest
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "MyBooksDBControllerTest")
            .Options;

        AppDbContext context;
        PublishersService publishersService;
        PublishersController publishersController;

        [OneTimeSetUp]
        public void Setup()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();
            publishersService = new PublishersService(context);
            publishersController = new PublishersController(publishersService, new NullLogger<PublishersController>());
        }

        [Test]
        public void GetAllPublishersController_Test()
        {
            IActionResult actionResult = publishersController.GetAllPublishers("name_desc", "Pub");
            Assert.That(actionResult,Is.TypeOf<OkObjectResult>());

            var actionResultData = (actionResult as OkObjectResult).Value as List<Publisher>;
            Assert.That(actionResultData.Last().Name, Is.EqualTo("Pub1"));
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

            context.SaveChanges();
        }
    }
}
