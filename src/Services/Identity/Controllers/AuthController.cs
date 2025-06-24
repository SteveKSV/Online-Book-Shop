using Identity.Managers.Interfaces;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDTO registerDto)
        {
            var result = await _authManager.SignUp(registerDto);
            if (result.StatusCode == "200")
                return Ok(result);
            if (result.StatusCode == "409")
                return Conflict(result);
            return StatusCode(500, result);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] LoginDTO loginDto)
        {
            var result = await _authManager.SignIn(loginDto);
            if (result.StatusCode == "200")
                return Ok(result);
            if (result.StatusCode == "401")
                return Unauthorized(result);
            return StatusCode(500, result);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var users = await _authManager.GetAllUsersAsync(pageNumber, pageSize);
                return Ok(users);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _authManager.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("updateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updateDto)
        {
            var result = await _authManager.UpdateUserAsync(updateDto);
            if (result.StatusCode == "200")
                return Ok(result);
            if (result.StatusCode == "401")
                return Unauthorized(result);
            return StatusCode(500, result);
        }

        [HttpDelete("{userId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                bool deleted = await _authManager.DeleteUserAsync(userId);
                if (!deleted)
                    return NotFound(new { message = "User not found." });

                return Ok(deleted);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
