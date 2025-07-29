using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.Requests;
using LandRegistrySystem_Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "1")]
    [ApiController]
    public class OrganizationInfoController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrganizationInfoController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/OrganizationInfo
        [HttpGet]
        public async Task<ActionResult<OrganizationInfoDto>> GetOrganizationInfo()
        {
            var org = await _db.OrganizationInfo.FirstOrDefaultAsync();
            if (org == null)
                return NotFound();

            var dto = new OrganizationInfoDto
            {
                Id = org.Id,
                Name = org.Name,
                Phone = org.Phone,
                Email = org.Email,
                Address = org.Address,
                Logo = org.Logo 
            };

            return Ok(dto);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateOrganizationInfo([FromForm] UpdateOrgRequest request)
        {
            var org = await _db.OrganizationInfo.FirstOrDefaultAsync();
            if (org == null)
                return NotFound();

            byte[]? logoBytes = null;

            if (request.Logo != null && request.Logo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await request.Logo.CopyToAsync(ms);
                    logoBytes = ms.ToArray();
                }
            }

            org.Update(request, logoBytes);

            await _db.SaveChangesAsync();
            return NoContent();
        }


    }
}
