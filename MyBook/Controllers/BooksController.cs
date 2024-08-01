using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBook.Data.Services;
using MyBook.Data.ViewModels;

namespace MyBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        public BooksService _bookService;

        public BooksController(BooksService bookService) 
        {
            _bookService = bookService;
        }


        #region [HttpPost("Add-Book")]
        [HttpPost("Add-Book")]
        public IActionResult AddBook([FromBody] BookVM book)
        {
            _bookService.AddBook(book);
            return Ok();
        }
        #endregion


        #region [HttpPut("Update-Book-ById/{id}")]
        [HttpPut("Update-Book-ById/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] BookVM book)
        {
            var updatedBook = _bookService.UpdateBookById(id, book);
            return Ok(updatedBook);
        }
        #endregion


        #region [HttpDelete("Delete-Book-ById/{id}")]
        [HttpDelete("Delete-Book-ById/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            var result = _bookService.DeleteBookById(id);
            if(!result)
            {
                return NotFound($"Book with id:{id} does not exist!");
            }    
            return Ok("Book is deleted sucessfully!!");
        }
        #endregion


        #region [HttpGet("Get-All-Books")]
        [HttpGet("Get-All-Books")]
        public IActionResult GetAllBooks()
        {
            var allBooks = _bookService.GetAllBooks();
            return Ok(allBooks);
        }
        #endregion


        #region [HttpGet("Get-Book-ById/{id}")]
        [HttpGet("Get-Book-ById/{id}")]
        public IActionResult GetBookById(int id)
        {
            var book = _bookService.GetBookById(id);
            return Ok(book);
        }
        #endregion

    }
}
