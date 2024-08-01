using Microsoft.AspNetCore.Mvc;
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

        public PublishersController(PublishersService publishersService)
        {
            _publishersService = publishersService;
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
            }
            else
            {
                return NotFound();
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
