using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBook.Data;
using MyBook.Data.Models;
using MyBook.Data.ViewModels.Authentication;

namespace MyBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVM payload)
        {

            var userExist = await _userManager.FindByEmailAsync(payload.Email);

            if (userExist != null)
            {
                return BadRequest($"User {payload.Email} already Exist!!");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                UserName = payload.UserName,
                Email = payload.Email,
                Custom = "Test",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(newUser, payload.Password);

            if (!result.Succeeded)
            {
                return BadRequest("User could not be created!!");
            }

            return Created(nameof(Register), $"User {payload.Email} is created!!");
        }
    }
}
