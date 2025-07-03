using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Domain.Requests;
using LandRegistrySystem_Infrastructure.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {

        private readonly IFileManagerRepository _fileManagerRepository;
        private readonly ICityRepository _cityRepository;

        public CitiesController(ICityRepository cityRepository, IFileManagerRepository fileManagerRepository)
        {
            _cityRepository = cityRepository;
            _fileManagerRepository = fileManagerRepository;

        }

        [Authorize(Roles = "1,2,3")]
        [HttpGet]
        public async Task<IActionResult> GetPaginatedCities([FromQuery] PaginationRequest request)
        {
            var result = await _cityRepository.GetCitites(request);
            return Ok(result);
        }

        [Authorize(Roles = "1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id)
        {
            var city = await _cityRepository.GetEntity(c => c.Id == id, tracked: false);
            if (city == null)
                return NotFound();

            var dto = new CityDto
            {
                Id = city.Id,
                CityNumber = city.CityNumber,
                Name = city.Name,
                ProjectsCount = city.Projects?.Count ?? 0
            };

            return Ok(dto);
        }

        [Authorize(Roles = "1,2")]
        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CreateCityRequest request)
        {
            var city = new City
            {
                CityNumber = request.CityNumber,
                Name = request.Name
            };
            await _cityRepository.CreateEntity(city);
            await _cityRepository.SaveChanges();

            _fileManagerRepository.CreateCityFolder(city.CityNumber.ToString());

            return Ok();
        }

        [Authorize(Roles = "1,2")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity([FromBody] UpdateCityRequest updateCityRequest)
        {
           

            var city = await _cityRepository.GetEntity(c => c.Id == updateCityRequest.Id);

            if (city == null)
                return NotFound($"City with id not found.");


            var currentUser = User.Identity.Name ?? null;
            city.Update(updateCityRequest, currentUser);

            // حفظ التغييرات في قاعدة البيانات
            await _cityRepository.SaveChanges();

            return Ok(); 
        }

        [Authorize(Roles = "1,2")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _cityRepository.GetEntity(c => c.Id == id);
            if (city == null)
                return NotFound();

            await _cityRepository.RemoveEntity(city);
            return Ok();
        }

        [Authorize(Roles = "1,2")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCities([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();

            var cities = await _cityRepository.GetEntities(c => ids.Contains(c.Id));
            if (cities == null || cities.Count == 0)
                return NotFound("");

            await _cityRepository.RemoveEntities(cities);
            return Ok();
        }

    }
}
