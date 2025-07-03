using LandRegistrySystem_Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public async Task<IActionResult> GetRoleUserCount()
        {
            var rolesWithUserCount = await _context.Roles
                .Select(role => new
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    UserCount = _context.Users.Count(u => u.RoleId == role.Id)
                })
                .ToListAsync();

            return Ok(rolesWithUserCount);
        }
    }
}
