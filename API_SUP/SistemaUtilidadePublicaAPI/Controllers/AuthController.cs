using Microsoft.AspNetCore.Mvc;
using SistemaUtilidadePublicaAPI.DTOs.Authentication;
using SistemaUtilidadePublicaAPI.Services.Authentication;

namespace SistemaUtilidadePublicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterUserDto dto)
        {
            var user = await _authService.RegisterAsync(dto);

            return Created(
                $"/api/users/{user.Id_User}",
                user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var user = await _authService.LoginService(dto);
            return Ok(user);
        }
    }
}