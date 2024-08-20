using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBook.Data;
using MyBook.Data.Models;
using MyBook.Data.ViewModels.Authentication;
using Newtonsoft.Json.Converters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVM payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide all required fields");
            }

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

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody]LoginVM payload)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Please provide all required fields");
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            
            if (user != null && await _userManager.CheckPasswordAsync(user,payload.Password))
            {
                var tokenValue = await GenerateJwtTokenAsync(user,"");

                return Ok(tokenValue);
            }

            return Unauthorized();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody]TokenRequestVM payload)
        {
            try
            {
                var result = await VerifyAndGenerateTokenAsync(payload);

                if (result == null) return BadRequest("Invalid token!!");
               
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<AuthResultVM> VerifyAndGenerateTokenAsync(TokenRequestVM payload)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var tokenInVerification = jwtTokenHandler.ValidateToken(payload.Token,_tokenValidationParameters,out var validatedToken);


            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);

                if (result == false) return null;
            }

            var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type ==
            JwtRegisteredClaimNames.Exp).Value);

            var expiryDate = UnixTimeStamptoDateTimeInUTC(utcExpiryDate);

            if (expiryDate > DateTime.UtcNow) throw new Exception("Token has not expired yet!!");


            var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token ==
            payload.RefreshToken);

            if (dbRefreshToken == null) throw new Exception("Refresh token does not exist in our DB!!");
            else
            {
                var jti = tokenInVerification.Claims.FirstOrDefault(n => n.Type == JwtRegisteredClaimNames.Jti).Value;

                if (dbRefreshToken.JwtId != jti) throw new Exception("Token does not match!!");

                if (dbRefreshToken.DateExpired < DateTime.UtcNow) throw new Exception("Your refresh token has expired!!!- please re-authenticate");

                if (dbRefreshToken.IsRevoked) throw new Exception("Refresh token is revoked!!");


                var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);

                var newTokenResponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);

                return await newTokenResponse;
            }
        }

        private DateTime UnixTimeStamptoDateTimeInUTC(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp);
            return dateTimeVal;
        }

        private async Task<AuthResultVM> GenerateJwtTokenAsync(ApplicationUser user, string existingRefreshToken)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.UtcNow.AddMinutes(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken();

            if (string.IsNullOrEmpty(existingRefreshToken))
            {
                refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.Now,
                    DateExpired = DateTime.UtcNow.AddMinutes(6),
                    Token = Guid.NewGuid().ToString() + "_" + Guid.NewGuid().ToString(),
                };

                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }

            

            var response = new AuthResultVM()
            {
                AccessToken = jwtToken,
                RefreshToken = (string.IsNullOrEmpty(existingRefreshToken)) ? refreshToken.Token : existingRefreshToken,
                ExpiresAt = token.ValidTo
            };

            return response;
        }
    }
}
