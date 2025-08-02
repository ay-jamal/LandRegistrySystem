using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Domain.Requests;
using LandRegistrySystem_Infrastructure.Context;
using LandRegistrySystem_Infrastructure.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IProjectRepository _projectRepository;
        private readonly IFileManagerRepository _fileManagerRepository;

        public ProjectsController(IProjectRepository projectRepository, IFileManagerRepository fileManagerRepository, AppDbContext db)
        {
            _db = db;
            _projectRepository = projectRepository;
            _fileManagerRepository = fileManagerRepository;

        }

        // GET: api/Projects
        [HttpGet]
        [Authorize(Roles = "1,2,3")]

        public async Task<ActionResult<List<ProjectDto>>> GetProjects(
            [FromQuery] int? CityId,
            [FromQuery] PaginationRequest request)
        {
            var projects = await _projectRepository.GetProjects(CityId, request);
            return Ok(projects);
        }

        [Authorize(Roles = "1,2,3")]

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _projectRepository.GetEntity(p => p.Id == id);
            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [Authorize(Roles = "1,2")]

        [HttpPost]
        public async Task<ActionResult> CreateProject([FromBody] ProjectCreateRequest request)
        {
            var EsistingProject = await _projectRepository.GetEntity(p => p.ProjectNumber == request.ProjectNumber);
            if (EsistingProject != null)
            {
                return BadRequest(new { Message = "رقم المشروع موجود مسبقا" });
            }
            var project = new Project
            {
                ProjectNumber = request.ProjectNumber,
                Name = request.Name,
                CityId = request.CityId,
            };

            // حفظ المشروع في قاعدة البيانات
            await _projectRepository.CreateEntity(project);
            await _projectRepository.SaveChanges();

            // إعادة تحميل المشروع مع المدينة المرتبطة
            project = await _db.Projects
                .Include(p => p.City)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            if (project?.City == null)
                return BadRequest("لم يتم العثور على المدينة المرتبطة بالمشروع.");

            var cityNumber = project.City.CityNumber;
            var projectNumber = project.ProjectNumber;
            _fileManagerRepository.CreateProjectFolder(cityNumber, projectNumber);


            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }


        [Authorize(Roles = "1,2")]
        // PUT: api/Projects/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProject(int id, [FromBody] ProjectUpdateRequest request)
        {
            var project = await _projectRepository.GetEntity(p => p.Id == id);
            if (project == null)
                return NotFound();

            var userName = User?.Identity?.Name ?? "Unknown";

            project.Update(request, userName);
            await _projectRepository.SaveChanges();

            return NoContent();
        }

        [Authorize(Roles = "1,2")]
        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            var project = await _projectRepository.GetEntity(p => p.Id == id);
            if (project == null)
                return NotFound();

            await _projectRepository.RemoveEntity(project);
            await _projectRepository.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "1,2")]
        [HttpDelete("DeleteMultiple")]
        public async Task<IActionResult> DeleteProjects([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();

            var projects = await _projectRepository.GetEntities(p => ids.Contains(p.Id));
            if (projects == null || projects.Count == 0)
                return NotFound();

            await _projectRepository.RemoveEntities(projects);
            return Ok();
        }

    }
}
