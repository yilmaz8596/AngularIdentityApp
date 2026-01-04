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
                Name = user.Name
            };

            return Ok(appUserDto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (await CheckEmailExistsAsync(registerDto.Email))
            {
                return Unauthorized(new APIResponse(
                    isSuccess: false,
                    statusCode: 400,
                    title: "Email Taken",
                    message: "The email is already taken.",
                    details: null,
                    errors: new List<string> { "Please use a different email address." }
                ));
            }

            if (await CheckNameExistsAsync(registerDto.Name))
            {
                return Unauthorized(new APIResponse(
                    isSuccess: false,
                    statusCode: 400,
                    title: "Username Taken",
                    message: "The username is already taken.",
                    details: null,
                    errors: new List<string> { "Please use a different username." }
                ));
            }

            var userToAdd = new AppUser
            {
                Name = registerDto.Name,
                UserName = registerDto.Name.ToLower(),
                Email = registerDto.Email,
                EmailConfirmed = true,
                LockoutEnabled = true,
            };

            var result = await _userManager.CreateAsync(userToAdd, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Registration successful");
        }

        [HttpGet("name-taken")]

        public async Task<IActionResult> IsNameTaken([FromQuery] string name)
        {
            if (await CheckNameExistsAsync(name))
            {
                return Ok(new { isTaken = true });
            }
            return Ok(new { isTaken = false });
        }

        [HttpGet("email-taken")]
        public async Task<IActionResult> IsEmailTaken([FromQuery] string email)
        {
            if (await CheckEmailExistsAsync(email))
            {
                return Ok(new { isTaken = true });
            }
            return Ok(new { isTaken = false });
        }

        [HttpGet("auth-status")]
        public IActionResult GetAuthStatus()
        {
            return Ok(new { isAuthenticated = User.Identity?.IsAuthenticated ?? false });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUserDto>> Login([FromBody] LoginDto loginDto)
        {

            var user = await _userManager.Users.Where(u => u.Email == loginDto.Email).FirstOrDefaultAsync();


            if (user is null)
            {
                return Unauthorized(new APIResponse(
                    isSuccess: false,
                    statusCode: 401,
                    title: "Invalid Credentials",
                    message: "The provided credentials are invalid.",
                    details: null,
                    errors: new List<string> { "Please check your email and password and try again." }
                ));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);

            if (!result.Succeeded)
            {
                RemoveJwtCookie();

                if (result.IsLockedOut)
                {
                    return Unauthorized(new APIResponse(
                        isSuccess: false,
                        statusCode: 401,
                        title: "Account Locked",
                        message: "The account is locked due to multiple failed login attempts.",
                        details: null,
                        errors: new List<string> { "Please try again later or reset your password." }
                    ));
                }
                return Unauthorized(new APIResponse(
                    isSuccess: false,
                    statusCode: 401,
                    title: "Invalid Credentials",
                    message: "The provided credentials are invalid.",
                    details: null,
                    errors: new List<string> { "Please check your email and password and try again." }
                ));
            }

            var jwtToken = _tokenService.CreateToken(user);
            SetJwtCookie(jwtToken);

            var appUserDto = new AppUserDto
            {
                Name = user.Name
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
                return Unauthorized(new APIResponse(
                    isSuccess: false,
                    statusCode: 401,
                    title: "User Not Found",
                    message: "The user could not be found.",
                    details: null,
                    errors: new List<string> { "Please log in again." }
                ));
            }

            var appUserDto = new AppUserDto
            {
                Name = user.Name
            };

            var jwtToken = _tokenService.CreateToken(user);
            SetJwtCookie(jwtToken);

            return CreateAppUserDto(appUserDto, jwtToken);
        }

        private AppUserDto CreateAppUserDto(AppUserDto appUserDto, string? jwtToken)
        {

            return new AppUserDto
            {
                Name = appUserDto.Name,
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


        private async Task<bool> CheckNameExistsAsync(string name)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == name.ToLower());
        }
    }
}
