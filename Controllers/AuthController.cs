using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NaviConnectWebApi.Data;
using NaviConnectWebApi.Models;
using NaviConnectWebApi.Services;
using NaviConnectWebApi.Services.UserService;

namespace NaviConnectWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment; //Добавляем сервис взаимодействия с файлами в рамках хоста
        

        public AuthController(IConfiguration configuration,
            IUserService userService, 
            DataContext context, 
            IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _userService = userService;
            _context = context;
            _environment = environment;
          
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            return Ok(userName);
        }
        
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromForm] UserRegisterRequest request)
        {
            if (_context.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest("User already exists.");
            }
            
            if (request.File.Length > 0)
            {
                if (!Directory.Exists(_environment.WebRootPath + "\\images\\"))
                {
                    Directory.CreateDirectory(_environment.WebRootPath + "\\images\\");
                }

                using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\images\\"+request.File.FileName))
                {
                    request.File.CopyTo(fileStream);
                    fileStream.Flush();
                    var photo = Path.GetFullPath(request.File.FileName);
                    
                     CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    
                     User user = new User
                     {
                         City = request.City,
                         Username = request.Username,
                         FirstName = request.FirstName,
                         Lastname = request.Lastname,
                         SurName = request.SurName,
                         AvatarPath =  photo,
                         PasswordHash = passwordHash,
                         PasswordSalt = passwordSalt
                     };
                                
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    return Ok(user);
                }
            }
            return Ok(request);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            
            if (user?.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            user.RefreshToken = CreateToken(user);

            // var refreshToken = GenerateRefreshToken();
            // SetRefreshToken(refreshToken);

            return Ok(user);
        }
        
        // [HttpPost("refresh-token")]
        // public async Task<ActionResult> RefreshToken()
        // {
        //     var refreshToken = Request.Cookies["refreshToken"];
        //     var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        //     
        //     if (!user.RefreshToken.Equals(refreshToken))
        //     {
        //         return Unauthorized("Invalid Refresh Token.");
        //     }
        //     else if(user.TokenExpires < DateTime.Now)
        //     {
        //         return Unauthorized("Token expired.");
        //     }
        //
        //     string token = CreateToken(user);
        //     var newRefreshToken = GenerateRefreshToken();
        //     SetRefreshToken(newRefreshToken);
        //
        //     
        //     return Ok(token);
        // }

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

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}

