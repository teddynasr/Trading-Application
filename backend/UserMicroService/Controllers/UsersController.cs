using Microsoft.AspNetCore.Mvc;
using UserMicroService.Models;
using UserMicroService.Services;

namespace UserMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            await _userService.AddUserAsync(user);
            var token = await _userService.GetTokenAsync(user.Id);

            var response = new UserCreationResponse
            {
                Id = user.Id,
                Token = token
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user)
        {
            if (id != user.Id) return BadRequest();
            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request){
            var UserId = await _userService.LoginAsync(request.Username, request.Password);
            if (UserId == null)
            {
                return Unauthorized();
            }
            
            var token = await _userService.GetTokenAsync(UserId.Value);
            return Ok(new { id = UserId.Value, token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] Guid userId)
        {
            await _userService.LogoutAsync(userId);
            return NoContent();
        }
    }
}
