using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers(string? SearchValue)
        {
            var users = await _userRepository.GetAllUsersAsync(SearchValue);
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetEntity(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "لم يتم العثور على المستخدم" });
            }
            return Ok(user);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest updateUserRequest)
        {
            var user = await _userRepository.GetEntity(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "لم يتم العثور على المستخدم" });
            }

            user.Update(updateUserRequest);
            await _userRepository.UpdateUserAsync(user);
            return Ok();
        }


        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateUserStatus(UpdateUserStatusRequest User)
        {
            var userToUpdate = await _userRepository.GetEntity(c => c.Id == User.Id);

            if (userToUpdate == null)
            {
                return NotFound(new { message = "المستخدم غير موجود" });
            }
            userToUpdate.UpdateStatus(User);
            await _userRepository.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetEntity(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "لم يتم العثور على المستخدم" });
            }

            int currentUserId = int.Parse(User.FindFirst("nameidentifier")?.Value ?? "0");
            if (id == currentUserId)
            {
                return BadRequest(new { message = "لا يمكنك حذف نفسك." });
            }

            // حماية آخر مدير نظام
            if (user.RoleId == 1)
            {
                var superAdmins = await _userRepository.GetEntities(u => u.RoleId == 1);
                int superAdminCount = superAdmins.Count();

                if (superAdminCount == 1)
                {
                    return BadRequest(new { message = "لا يمكن حذف مدير النظام الأخير." });
                }
            }

            await _userRepository.RemoveEntity(user);

            return Ok();
        }


    }
}
