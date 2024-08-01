using Microsoft.AspNetCore.Mvc;
using MyBook.Data.Services;
using MyBook.Data.ViewModels;

namespace MyBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private PublishersService _publishersService;

        public PublishersController(PublishersService publishersService)
        {
            _publishersService = publishersService; 
        }

        #region [HttpPost("Add-Publisher")]
        [HttpPost("Add-Publisher")]
        public IActionResult AddPublisher([FromBody] PublisherVM publisher)
        {
            _publishersService.AddPublisher(publisher);
            return Ok();
        }
        #endregion
    }
}
