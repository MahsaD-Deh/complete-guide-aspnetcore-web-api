using Microsoft.AspNetCore.Mvc;
using MyBook.ActionResults;
using MyBook.Data.Services;
using MyBook.Data.ViewModels;
using MyBook.Exceptions;

namespace MyBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private PublishersService _publishersService;
        private ILogger<PublishersController> _logger;

        public PublishersController(PublishersService publishersService, ILogger<PublishersController> logger)
        {
            _publishersService = publishersService;
            _logger = logger;
        }


        [HttpGet("GetAllPublishers")]
        public IActionResult GetAllPublishers(string sortBy, string searchString)
        {
            try
            {
                _logger.LogInformation("This is just a log in GetAllPublishers()");
                var result = _publishersService.GetAllPublishers(sortBy, searchString);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("Sorry, we could not load the publishers!!");
            }
            
        }
        #region [HttpPost("Add-Publisher")]
        [HttpPost("Add-Publisher")]
        public IActionResult AddPublisher([FromBody] PublisherVM publisher)
        {
            try
            {
                var newPublisher = _publishersService.AddPublisher(publisher);
                return Created(nameof(AddPublisher), newPublisher);
            }
            catch(PublisherNameException ex)
            {
                return BadRequest($"{ex.Message}, publisherName: {ex.PublisherName}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        #endregion


        #region [HttpGet("GetPublisherWithBooksAndAuthors/{id}")
        [HttpGet("GetPublisherWithBooksAndAuthors/{id}")]
        public IActionResult GetPublisherData(int id)
        {
            var response = _publishersService.GetPublisherData(id);
            return Ok(response);
        }
        #endregion


        #region [HttpGet("GetPublisherById/{id}")
        [HttpGet("GetPublisherById/{id}")]
        public IActionResult GetPublisherById(int id)
        {
            // throw new Exception("This is the exception that will be handled by Middleware");

            var response = _publishersService.GetPublisherById(id);
            if (response != null)
            {
                return Ok(response);
                /*
                var responseObj = new CustomActionResultVM()
                {
                    Publisher = response,
                };
                return new CustomActionResult(responseObj);
                */
            }
            else
            {
                return NotFound();

                /*
                var responseObj = new CustomActionResultVM()
                {
                    Exception = new Exception("This is coming from publishers controller!!"),
                };
                return new CustomActionResult(responseObj);
                */
            }
        }
        #endregion


        #region [HttpDelete("DeletePublisherById/{id}")]
        [HttpDelete("DeletePublisherById/{id}")]
        public IActionResult DeletePublisher(int id)
        {
            try
            {
                _publishersService.DeletePublisher(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

    }
}
