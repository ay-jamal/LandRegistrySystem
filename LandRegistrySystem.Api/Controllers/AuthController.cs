using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [Authorize(Roles = "1")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            if (await _authRepository.UserExists(userDto.Username))
            {
                return BadRequest(new { message = "اسم المستخدم موجود بالفعل" });
            }

            var user = new User
            {
                Username = userDto.Username,
                FullName = userDto.FullName,
                Email = userDto.Email,
                RoleId = userDto.RoleId,
                Adress = userDto.Adress,
                Phone = userDto.Phone,
                IsActive = userDto.IsActive
            };

            await _authRepository.Register(user, userDto.Password);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            var userResponse = await _authRepository.Login(userDto.Username, userDto.Password);

            if (userResponse == null)
                return Unauthorized(new { message = "بيانات تسجيل الدخول غير صحيحة أو الحساب غير نشط" });

            return Ok(userResponse);
        }


    }
}
