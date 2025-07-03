using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<OrganizationInfo>> GetOrganizationInfo()
        {
            var org = await _db.OrganizationInfo.FirstOrDefaultAsync();
            if (org == null)
                return NotFound();

            // نخفي الشعار هنا حتى لا يسبب بطء في API (أو يمكن إرجاعه حسب رغبتك)
            org.Logo = null;
            return org;
        }

        // GET: api/OrganizationInfo/logo
        [HttpGet("logo")]
        public async Task<IActionResult> GetLogo()
        {
            var org = await _db.OrganizationInfo.FirstOrDefaultAsync();
            if (org == null || org.Logo == null)
                return NotFound();

            // إرجاع الشعار كصورة مباشرة
            return File(org.Logo, "image/png"); // أو image/jpeg حسب نوع الصورة
        }

        // PUT: api/OrganizationInfo
        [HttpPut]
        public async Task<IActionResult> UpdateOrganizationInfo([FromBody] OrganizationInfo model)
        {
            var org = await _db.OrganizationInfo.FirstOrDefaultAsync();
            if (org == null)
                return NotFound();

            org.Name = model.Name;
            org.Phone = model.Phone;
            org.Email = model.Email;
            org.Address = model.Address;

            // لا تحدث الشعار هنا، لأنه عادة يتم رفعه من Endpoint مستقل

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/OrganizationInfo/logo
        [HttpPost("logo")]
        public async Task<IActionResult> UploadLogo([FromForm] IFormFile file)
        {
            var org = await _db.OrganizationInfo.FirstOrDefaultAsync();
            if (org == null)
                return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("لا يوجد ملف شعار مرفوع");

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                org.Logo = ms.ToArray();
            }

            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
