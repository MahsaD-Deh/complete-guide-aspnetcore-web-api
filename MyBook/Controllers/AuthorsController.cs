using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBook.Data.Services;
using MyBook.Data.ViewModels;
using MyBook.Data.ViewModels.Authentication;

namespace MyBook.Controllers
{
    [Authorize(Roles = UserRoles.Author)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private AuthorsService _authorsService;

        public AuthorsController(AuthorsService authorsService)
        {
            _authorsService = authorsService; 
        }

        #region [HttpPost("Add-Author")]
        [HttpPost("Add-Author")]
        public IActionResult AddAuthor([FromBody] AuthorVM author)
        {
            _authorsService.AddAuthor(author);
            return Ok();
        }
        #endregion

        [HttpGet("GetAuthorWithBook-ById/{id}")]
        public IActionResult GetAuthorWithBooks(int id) 
        {
            var response = _authorsService.GetAuthorWithBooks(id);
            return Ok(response);
        }

    }
}
