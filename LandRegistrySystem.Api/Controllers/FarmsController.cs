using BarcodeStandard;
using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Domain.Requests;
using LandRegistrySystem_Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using ZXing;
using ZXing.SkiaSharp;
using SkiaSharp;
using Microsoft.AspNetCore.Authorization;
using LandRegistrySystem_Infrastructure.Implementation;
using System.Formats.Tar;
using LandRegistrySystem_Domain.Reports;
using QuestPDF.Fluent;
using LandRegistrySystem_Infrastructure.Services;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    public class FarmsController : ControllerBase
    {
        private readonly string _rootPath;
        private readonly AppDbContext _dbContext;
        private readonly IFileManagerRepository _fileManager;
        private readonly IFarmRepository _farmRepository;
        private readonly IProjectRepository _projectRepository;

        private readonly IOwnerRepository _ownerRepository;

        public FarmsController(
            AppDbContext dbContext,
            IFarmRepository farmRepository,
            IConfiguration configuration,
            IProjectRepository projectRepository,
            IFileManagerRepository fileManager
,

            IOwnerRepository ownerRepository
            )
        {
            _dbContext = dbContext;
            _farmRepository = farmRepository;
            _rootPath = configuration["FileStorage:RootPath"] ?? "C:\\LandFiles\\";
            _projectRepository = projectRepository;
            _fileManager = fileManager;
            _ownerRepository = ownerRepository;
        }

        [Authorize(Roles = "1,2,3")]

        // GET: api/Farms?page=1&pageSize=10&searchValue=abc
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<FarmDto>>> GetFarms(
          [FromQuery] int? cityId,
          [FromQuery] int? projectId,
          [FromQuery] PaginationRequest paginationRequest)
        {
            var result = await _farmRepository.GetFarms(cityId, projectId, paginationRequest);
            return Ok(result);
        }
        [Authorize(Roles = "1,2,3")]

        // GET: api/Farms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FarmDto>> GetFarm(int id)
        {
            var farm = await _farmRepository.GetEntity(f => f.Id == id, tracked: false);
            if (farm == null)
                return NotFound();

            return Ok(farm);
        }

        [Authorize(Roles = "1,2,3")]


        [HttpGet("generate/{farmId}")]
        public async Task<IActionResult> GenerateFarmBarcode(int farmId)
        {
            // جلب المزرعة مع المشروع والمدينة
            var farm = await _dbContext.Farms
                .Include(f => f.Project)
                .ThenInclude(p => p.City)
                .FirstOrDefaultAsync(f => f.Id == farmId);

            if (farm == null)
                return NotFound("المزرعة غير موجودة");

            // توليد الباركود ديناميكياً
            string barcodeContent = $"{farm.Project.City.CityNumber}{farm.Project.ProjectNumber}{farm.FarmNumber}";

            // إنشاء الباركود باستخدام ZXing.Net
            var writer = new ZXing.BarcodeWriter<SKBitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 10
                },
                Renderer = new ZXing.SkiaSharp.Rendering.SKBitmapRenderer() // ✅ هذا هو المطلوب

            };

            // إنشاء الباركود مباشرة كـ SKBitmap
            using (var bitmap = writer.Write(barcodeContent))
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                return File(data.ToArray(), "image/png", $"farm_{farmId}_barcode.png");
            }
        }

        [Authorize(Roles = "1,2,3")]
        [HttpGet("generate/{farmId}/qr")]
        public async Task<IActionResult> GetFarmQrCode(int farmId)
        {
            // جلب المزرعة مع المشروع والمدينة
            var farm = await _dbContext.Farms
                .Include(f => f.Project)
                .ThenInclude(p => p.City)
                .FirstOrDefaultAsync(f => f.Id == farmId);

            if (farm == null)
                return NotFound("المزرعة غير موجودة");

            // توليد الباركود ديناميكياً
            string barcodeContent = $"{farm.Project.City.CityNumber}{farm.Project.ProjectNumber}{farm.FarmNumber}";
            return Ok(barcodeContent);
        }

        [Authorize(Roles = "1,2,3")]
        [HttpGet("{farmId}/report")]
        public async Task<IActionResult> GenerateFarmReport(int farmId)
        {
            var farm = await _dbContext.Farms
                .Include(f => f.Project)
                .Include(f => f.Owner)
                .Include(f => f.Boundaries)
                .FirstOrDefaultAsync(f => f.Id == farmId);

            if (farm == null)
                return NotFound("المزرعة غير موجودة.");


            var documents = await _dbContext.FarmDocuments
                .Where(d => d.FarmId == farmId)
                .ToListAsync();

            var organization = await _dbContext.OrganizationInfo.FirstOrDefaultAsync();

            var report = new FarmReportDocument
            {
                Farm = farm,
                Documents = documents,
               Organization = organization

            };

            var pdfBytes = report.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Farm_{farm.FarmNumber}_Report.pdf");
        }

        [Authorize(Roles = "1,2")]
        [HttpPost]
        public async Task<ActionResult> CreateFarm([FromBody] CreateFarmRequest request)
        {
            // 1. تحقق من وجود المشروع
            var project = await _projectRepository.GetEntity(p => p.Id == request.ProjectId);
            if (project == null)
            {
                return BadRequest(new { Message = "المشروع غير موجود." });
            }

            // 2. تحقق هل رقم المزرعة مكرر
            var existingFarm = await _farmRepository.GetEntity(
            f => f.FarmNumber == request.FarmNumber && f.ProjectId == request.ProjectId);

            if (existingFarm != null)
            {
                return BadRequest(new { Message = "رقم المزرعة موجود مسبقاً في هذا المشروع." });
            }

            // تحقق من وجود المالك
            var owner = await _ownerRepository.GetEntity(o => o.Id == request.OwnerId);
            if (owner == null)
                return BadRequest(new { Message = "المالك غير موجود." });

            // 3. إنشاء المزرعة
            var farm = new Farm
            {
                Area = request.Area,
                ProjectId = request.ProjectId,
                OwnerId = request.OwnerId,
                FarmNumber = request.FarmNumber,
                Boundaries = new FarmBoundaries
                {
                    North = request.North,
                    South = request.South,
                    East = request.East,
                    West = request.West
                }
            };

            await _farmRepository.CreateEntity(farm);
            await _farmRepository.SaveChanges();

            // 4. جلب المشروع مع المدينة لمسار الملفات (هنا تحتاج بيانات المدينة)
            var projectWithCity = await _projectRepository.GetByIdWithCityAsync(request.ProjectId);
            if (projectWithCity == null)
            {
                // يفترض هذا لا يحصل لأنه تحققنا فوق، لكن احتياطاً
                return BadRequest(new { Message = "المشروع غير موجود." });
            }

            // 5. تحديد مسار مجلد المشروع داخل مجلد المدينة
            string folderPath = Path.Combine(
                _rootPath,
                projectWithCity.City.CityNumber,                              // "01"
                $"{projectWithCity.City.CityNumber}{projectWithCity.ProjectNumber}",
                $"{projectWithCity.City.CityNumber}{projectWithCity.ProjectNumber}{request.FarmNumber}"
            );

            // 6. ربط ملفات المزرعة إن وجدت في المسار
            if (Directory.Exists(folderPath))
            {
                var filePaths = Directory.GetFiles(folderPath);

                foreach (var path in filePaths)
                {
                    var farmFile = new FarmDocument
                    {
                        FarmId = farm.Id,
                        FileName = Path.GetFileName(path),
                        FilePath = path,
                        UploadedAt = DateTime.UtcNow
                    };

                    await _dbContext.FarmDocuments.AddAsync(farmFile);
                }

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"Folder not found: {folderPath}");
            }

            return Ok(new { FarmId = farm.Id });
        }

        [Authorize(Roles = "1,2")]

        [HttpPut]
        public async Task<IActionResult> UpdateFarm([FromBody]  UpdateFarmRequest request)
        {
            // جلب المزرعة مع الحدود المرتبطة بها
            var farm = await _dbContext.Farms
                .Include(f => f.Boundaries)
                .FirstOrDefaultAsync(f => f.Id == request.Id);

            if (farm == null)
                return NotFound();

            var project = await _projectRepository.GetEntity(p => p.Id == request.ProjectId);
            if (project == null)
            {
                return BadRequest(new { Message = "المشروع غير موجود." });
            }

            // تحقق من وجود المالك
            var owner = await _ownerRepository.GetEntity(o => o.Id == request.OwnerId);
            if (owner == null)
                return BadRequest(new { Message = "المالك غير موجود." });

            var userName = User?.Identity?.Name ?? "Unknown";

            // تنفيذ التحديث
            farm.Update(request, userName);

            await _farmRepository.SaveChanges();

            return NoContent();
        }

        [Authorize(Roles = "1,2")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            var farm = await _dbContext.Farms
                .Include(f => f.Project)
                    .ThenInclude(p => p.City)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (farm == null)
                return NotFound("المزرعة غير موجودة");

            // حذف ملفات FarmDocuments
            var farmDocuments = await _dbContext.FarmDocuments
                .Where(d => d.FarmId == farm.Id)
                .ToListAsync();
            _dbContext.FarmDocuments.RemoveRange(farmDocuments);

            // حذف المجلد من السيرفر
            string folderPath = Path.Combine(
                _rootPath,
                farm.Project.City.CityNumber,
                $"{farm.Project.City.CityNumber}{farm.Project.ProjectNumber}",
                $"{farm.Project.City.CityNumber}{farm.Project.ProjectNumber}{farm.FarmNumber}"
            );

            if (Directory.Exists(folderPath))
            {
                try
                {
                    Directory.Delete(folderPath, recursive: true);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"فشل حذف مجلد المزرعة من السيرفر: {ex.Message}");
                }
            }

            // حذف المزرعة
            _dbContext.Farms.Remove(farm);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "1,2")]

        [HttpDelete]
        public async Task<IActionResult> DeleteFarms([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("لم يتم إرسال أي معرفات.");

            var farms = await _dbContext.Farms
                .Include(f => f.Project)
                    .ThenInclude(p => p.City)
                .Where(f => ids.Contains(f.Id))
                .ToListAsync();

            if (farms == null || farms.Count == 0)
                return NotFound("لم يتم العثور على أي مزارع.");

            foreach (var farm in farms)
            {
                var farmDocuments = await _dbContext.FarmDocuments
                    .Where(d => d.FarmId == farm.Id)
                    .ToListAsync();
                _dbContext.FarmDocuments.RemoveRange(farmDocuments);

                string folderPath = Path.Combine(
                    _rootPath,
                    farm.Project.City.CityNumber,
                    $"{farm.Project.City.CityNumber}{farm.Project.ProjectNumber}",
                    $"{farm.Project.City.CityNumber}{farm.Project.ProjectNumber}{farm.FarmNumber}"
                );

                if (Directory.Exists(folderPath))
                {
                    try
                    {
                        Directory.Delete(folderPath, recursive: true);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"فشل حذف مجلد المزرعة (ID: {farm.Id}) من السيرفر: {ex.Message}");
                    }
                }
            }

            _dbContext.Farms.RemoveRange(farms);
            await _dbContext.SaveChangesAsync();

            return Ok("تم حذف المزارع والملفات المرتبطة بها بنجاح.");
        }


      

    }
}
