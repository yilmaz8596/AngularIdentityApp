using API.DTOs;
using API.Extensions;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IConfiguration config) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IConfiguration _config = config;

        [Authorize]
        [HttpGet("auth-user")]
        public async Task<IActionResult> GetAuthUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var appUserDto = new AppUserDto
            {
                Username = user.UserName
            };

            return Ok(appUserDto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (await CheckEmailExistsAsync(registerDto.Email))
            {
                return BadRequest("Email is already in use");
            }

            if (await CheckUsernameExistsAsync(registerDto.Username))
            {
                return BadRequest("Username is already taken");
            }

            var userToAdd = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(userToAdd, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserDto>> Login([FromBody] LoginDto loginDto)
        {

            var user = await _userManager.Users.Where(u => u.Email == loginDto.Email).FirstOrDefaultAsync();


            if (user is null)
            {
                return BadRequest("User not found");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                RemoveJwtCookie();
                return Unauthorized("Invalid username or password");
            }

            var jwtToken = _tokenService.CreateToken(user);
            SetJwtCookie(jwtToken);

            var appUserDto = new AppUserDto
            {
                Username = user.UserName
            };

            return CreateAppUserDto(appUserDto, jwtToken);
        }

        [Authorize]
        [HttpPost("logout")]

        public IActionResult Logout()
        {
            RemoveJwtCookie();
            return Ok("Logged out successfully");
        }

        [Authorize]
        [HttpGet("refresh-user")]
        public async Task<ActionResult<AppUserDto>> RefreshUser()
        {
            var user = await _userManager.Users.Where(u => u.Id == User.GetUserId()).FirstOrDefaultAsync();

            if (user is null)
            {
                RemoveJwtCookie();
                return Unauthorized("User not found");
            }

            var appUserDto = new AppUserDto
            {
                Username = user.UserName
            };

            var jwtToken = _tokenService.CreateToken(user);
            SetJwtCookie(jwtToken);

            return CreateAppUserDto(appUserDto, jwtToken);
        }

        private AppUserDto CreateAppUserDto(AppUserDto appUserDto, string? jwtToken)
        {

            return new AppUserDto
            {
                Username = appUserDto.Username,
                JWT = jwtToken
            };
        }

        private void SetJwtCookie(string jwtToken)
        {
            var cookieOptions = new CookieOptions
            {
                IsEssential = true,
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:ExpireDays"])),
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
            };
            Response.Cookies.Append("jwtToken", jwtToken, cookieOptions);
        }

        private void RemoveJwtCookie()
        {
            Response.Cookies.Delete("jwtToken");
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        private async Task<bool> CheckUsernameExistsAsync(string username)
        {
            return await _userManager.FindByNameAsync(username) != null;
        }
    }
}
