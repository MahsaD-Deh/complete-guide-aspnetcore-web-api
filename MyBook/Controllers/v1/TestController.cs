using Microsoft.AspNetCore.Mvc;

namespace MyBook.Controllers.v1
{
    [ApiVersion("1.0")]
    //[Route("api/[controller]")]
    [Route("api/v{version:apiversion}/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet("get-test-data")]
        public IActionResult Get()
        {
            return Ok("This is TestController V1.0!!");
        }

        /*
        [HttpGet("get-test-data"), MapToApiVersion("1.2")]
        public IActionResult GetV12()
        {
            return Ok("This is TestController V1.2!!");
        }

        [HttpGet("get-test-data"), MapToApiVersion("1.9")]
        public IActionResult GetV19()
        {
            return Ok("This is TestController V1.9!!");
        }
        */
    }
}
