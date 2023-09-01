using SocialMediaApplication.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using SocialMediaApplication.Models;
using SocialMediaApplication.DTO;
using System.Text;
using SocialMediaApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace SocialMediaApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static Users users = new Users();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private DataContext _context;

        public AuthController(IConfiguration configuration, IUserService userService, DataContext context)
        {
            _configuration = configuration;
            _userService = userService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Users>>> Get()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpGet("usernames")]
        public async Task<ActionResult<List<string>>> GetUsernames()
        {
            var usernames = await _context.Users.Select(user => user.username).ToListAsync();
            return Ok(usernames);
        }

        [HttpPost("register")]
        public async Task<ActionResult<Users>> Register(RegisterDto request)
        {
            request.Password= hashPassword(request.Password);
            users.userId = 0;
            users.username = request.UserName;
            users.name = request.name;
            users.surname = request.surname;
            users.email = request.Email;
            users.password = request.Password;
            users.roleName = request.Role;
            hashPassword(request.Password);

            _context.Users.Add(users);
            await _context.SaveChangesAsync();

            return Ok(users);
        }



        [HttpPost("login")]
        public async Task<ActionResult<object>> Login(LoginDto request)
        {
            try
            {
                string password = hashPassword(request.password);
                var dbUser = _context.Users.FirstOrDefault(u => u.username == request.username && u.password == password);
                if (dbUser == null)
                {
                    return BadRequest("Username or password is incorrect");
                }

                string token = CreateToken(dbUser);
                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(refreshToken, dbUser); // Update the user's RefreshToken and token-related properties
                await _context.SaveChangesAsync(); // Save changes to the database

                var response = new { token, role = dbUser.roleName, userId = dbUser.userId }; // Include userId in the response
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, Users user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }


        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            users.RefreshToken = newRefreshToken.Token;
            users.TokenCreated = newRefreshToken.Created;
            users.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(Users user)
        {
            List<Claim> claims = new List<Claim>
    {
        new Claim("userId", user.userId.ToString()),
        new Claim(ClaimTypes.SerialNumber, user.userId.ToString()),
        new Claim("username", user.username),
        new Claim(ClaimTypes.GivenName, user.username),
        new Claim("name", user.name),
        new Claim(ClaimTypes.Name, user.name),
        new Claim("surname", user.surname),
        new Claim(ClaimTypes.Surname, user.surname),
        new Claim("role", user.roleName),
        new Claim(ClaimTypes.Role, user.roleName),
        new Claim("email", user.email),
        new Claim(ClaimTypes.Email, user.email)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        static string hashPassword(string password)
        {
            var sha = SHA256.Create();
            var asByteArray = Encoding.Default.GetBytes(password);
            var hashedPassword = sha.ComputeHash(asByteArray);
            return Convert.ToBase64String(hashedPassword);
        }

    
    }
}
