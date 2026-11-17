
using LibrarayManagement.Interfaces;
using LibraryManagement.DTOs;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        // USER REGISTRATION
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // 1. Check whether user already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return BadRequest("Email is already registered.");
            }

            // 2. Create ApplicationUser object
            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email
            };

            // 3. Create user and hash password
            var result = await _userManager.CreateAsync(
                user,
                dto.Password    
            );

            // 4. Check whether user creation succeeded
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(new
                {
                    Errors = errors
                });
            }

            return Ok(new
            {
                Message = "User registered successfully!",
                UserId = user.Id,
                Email = user.Email
            });
        }


        // USER LOGIN + JWT GENERATION
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // 1. Find user using email
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // 2. Verify password
            var isPasswordValid = await _userManager.CheckPasswordAsync(
                user,
                dto.Password
            );

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            // 3. Retrieve user roles
            var roles = await _userManager.GetRolesAsync(user);

            // 4. Generate JWT token
            var token = _tokenService.CreateToken(user, roles);

            // 5. Return token to client
            return Ok(new
            {
                Message = "Login successful!",
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Token = token
            });
        }


        // GET CURRENT AUTHENTICATED USER
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            // Retrieve UserId from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Retrieve Email from JWT claims
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new
            {
                Message = "You are authenticated!",
                UserId = userId,
                Email = email
            });
        }
    }
}
