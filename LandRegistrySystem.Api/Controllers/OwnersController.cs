using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Domain.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnersController(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        [Authorize(Roles = "1,2,3")]

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<OwnerDto>>> GetOwners([FromQuery] PaginationRequest paginationRequest)
        {
            var owners = await _ownerRepository.GetOwners(paginationRequest);
            return Ok(owners);
        }

        [Authorize(Roles = "1,2,3")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OwnerDto>> GetOwner(int id)
        {
            var owner = await _ownerRepository.GetEntity(o => o.Id == id, tracked: false);
            if (owner == null)
                return NotFound();

            var dto = new OwnerDto
            {
                Id = owner.Id,
                FullName = owner.FullName,
                NationalId = owner.NationalId,
                PhoneNumber = owner.PhoneNumber
            };

            return Ok(dto);
        }

        [Authorize(Roles = "1,2")]
        [HttpPost]
        public async Task<ActionResult> CreateOwner(CreateOwnerRequest request)
        {
            var ExsistingOwner = await this._ownerRepository.GetEntity(o => o.NationalId == request.NationalId);
            if (ExsistingOwner != null)
            {
                return BadRequest(
                    new
                    {
                        Meassage = "الرقم الوطني مرتبط بمالك اخر بالفعل"
                    }
                    );
            }

            var owner = new Owner
            {
                FullName = request.FullName,
                NationalId = request.NationalId,
                PhoneNumber = request.PhoneNumber
            };

            await _ownerRepository.CreateEntity(owner);
            return CreatedAtAction(nameof(GetOwner), new { id = owner.Id }, owner);
        }

        [Authorize(Roles = "1,2")]

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOwner(UpdateOwnerRequest request)
        {
            var owner = await _ownerRepository.GetEntity(o => o.Id == request.Id);
            if (owner == null)
                return NotFound();

            if (owner.IsProtected)
                return BadRequest(new { Message = "لا يمكن تعديل بيانات المالك الافتراضي." });

            owner.Update(request);
            await _ownerRepository.SaveChanges();

            return NoContent();
        }

        [Authorize(Roles = "1,2")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOwner(int id)
        {
            var owner = await _ownerRepository.GetEntity(o => o.Id == id);
            if (owner == null)
                return NotFound();

            if (owner.IsProtected)
                return BadRequest(new { Message = "لا يمكن حذف المالك الافتراضي." });

            await _ownerRepository.RemoveEntity(owner);
            return NoContent();
        }

        [Authorize(Roles = "1,2")]

        [HttpDelete]
        public async Task<IActionResult> DeleteOwners([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();

            var owners = await _ownerRepository.GetEntities(o => ids.Contains(o.Id));
            if (owners == null || owners.Count == 0)
                return NotFound();

            // التأكد أنه لا يوجد أي مالك محمي في القائمة
            if (owners.Any(o => o.IsProtected))
                return BadRequest(new { Message = "لا يمكن حذف المالك الافتراضي أو أي مالك محمي." });

            await _ownerRepository.RemoveEntities(owners);
            return Ok();
        }


    }
}
