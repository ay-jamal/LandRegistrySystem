using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LandRegistrySystem_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class FarmDocumentsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IFileManagerRepository _fileManager;

        public FarmDocumentsController(AppDbContext dbContext, IFileManagerRepository fileManager)
        {
            _dbContext = dbContext;
            _fileManager = fileManager;
        }



        [HttpPost("upload")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> UploadFiles([FromForm] int farmId, [FromForm] List<IFormFile> files)
        {
            var farm = await _dbContext.Farms
                .Include(f => f.Project)
                .ThenInclude(p => p.City)
                .FirstOrDefaultAsync(f => f.Id == farmId);

            if (farm == null)
                return NotFound("المزرعة غير موجودة");

            var cityNumber = farm.Project.City.CityNumber;
            var projectNumber = farm.Project.ProjectNumber;
            var farmNumber = farm.FarmNumber;

            // إنشاء مجلد المشروع: {CityNumber}/{ProjectNumber}
            _fileManager.CreateProjectFolder(cityNumber, projectNumber);

            // إنشاء مجلد المزرعة: {CityNumber}/{ProjectNumber}/{FarmNumber}
            var farmFolderPath = _fileManager.CreateFarmFolder(cityNumber, projectNumber, farmNumber);

            foreach (var file in files)
            {
                var filePath = await _fileManager.SaveFileAsync(farmFolderPath, file);

                var document = new FarmDocument
                {
                    FarmId = farmId,
                    FileName = file.FileName,
                    FilePath = filePath,
                    UploadedAt = DateTime.UtcNow
                };

                await _dbContext.FarmDocuments.AddAsync(document);
            }

            await _dbContext.SaveChangesAsync();
            return Ok( new
            {
                Message = "تم رفع الملفات بنجاح"
            });
        }

        [HttpGet("{farmId}")]
        [Authorize(Roles = "1,2,3")]

        public async Task<IActionResult> GetFarmDocuments(int farmId)
        {
            var documents = await _dbContext.FarmDocuments
                .Where(d => d.FarmId == farmId)
                .Select(d => new { d.Id, d.FileName, d.UploadedAt })
                .ToListAsync();

            return Ok(documents);
        }

        [HttpGet("download/{documentId}")]
        [Authorize(Roles = "1,2,3")]

        public async Task<IActionResult> DownloadFile(int documentId)
        {
            var document = await _dbContext.FarmDocuments.FindAsync(documentId);
            if (document == null || !System.IO.File.Exists(document.FilePath))
                return NotFound();

            var fileBytes = await _fileManager.GetFileAsync(document.FilePath);
            return File(fileBytes, "application/octet-stream", document.FileName);
        }


        [HttpGet("preview/{documentId}")]
        [Authorize(Roles = "1,2,3")]
        public async Task<IActionResult> PreviewFile(int documentId)
        {
            var document = await _dbContext.FarmDocuments.FindAsync(documentId);
            if (document == null || !System.IO.File.Exists(document.FilePath))
                return NotFound();

            var fileBytes = await _fileManager.GetFileAsync(document.FilePath);

            // استخراج الامتداد
            var extension = Path.GetExtension(document.FilePath).ToLowerInvariant();

            // تعيين نوع المحتوى بناءً على الامتداد
            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                _ => "application/octet-stream" // نوع افتراضي إذا لم يتم التعرف عليه
            };

            return File(fileBytes, contentType);
        }

        [HttpDelete("{documentId}")]
        [Authorize(Roles = "1,2")]

        public async Task<IActionResult> DeleteFile(int documentId)
        {
            var document = await _dbContext.FarmDocuments.FindAsync(documentId);
            if (document == null)
                return NotFound("السجل غير موجود في قاعدة البيانات.");

            bool fileExisted = System.IO.File.Exists(document.FilePath);
            if (fileExisted)
            {
                try
                {
                    _fileManager.DeleteFile(document.FilePath);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"حدث خطأ أثناء حذف الملف: {ex.Message}");
                }
            }

            _dbContext.FarmDocuments.Remove(document);
            await _dbContext.SaveChangesAsync();

            if (!fileExisted)
                return Ok(new  { Meassage ="تم حذف السجل من قاعدة البيانات. الملف لم يكن موجودًا على السيرفر." });

            return Ok(new { Meassage =  "تم حذف الملف والسجل بنجاح." });
        }



        [HttpPut("replace/{documentId}")]
        [Authorize(Roles = "1,2")]

        public async Task<IActionResult> ReplaceFile(int documentId, IFormFile newFile)
        {
            var document = await _dbContext.FarmDocuments
                .Include(d => d.Farm)
                .ThenInclude(f => f.Project)
                .ThenInclude(p => p.City)
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
                return NotFound();

            var cityNumber = document.Farm.Project.City.CityNumber;
            var projectNumber = document.Farm.Project.ProjectNumber;
            var farmNumber = document.Farm.FarmNumber;

            // حذف الملف القديم
            _fileManager.DeleteFile(document.FilePath);

            // تأكيد وجود مجلد المشروع والمزرعة
            _fileManager.CreateProjectFolder(cityNumber, projectNumber);
            var farmFolderPath = _fileManager.CreateFarmFolder(cityNumber, projectNumber, farmNumber);

            var newPath = await _fileManager.SaveFileAsync(farmFolderPath, newFile);

            document.FileName = newFile.FileName;
            document.FilePath = newPath;
            document.UploadedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return Ok(new
            {
                Meassage = "تم استبدال الملف"
            });
        }

        [HttpGet("tree")]
        [Authorize(Roles = "1,2,3")]

        public async Task<IActionResult> GetDocumentTree()
        {
            var cities = await _dbContext.Citites
                .Include(c => c.Projects)
                    .ThenInclude(p => p.Farms)
                        .ThenInclude(f => f.Documents)
                .Include(c => c.Projects)
                    .ThenInclude(p => p.Farms)
                        .ThenInclude(f => f.Boundaries)
                .ToListAsync();

            var tree = cities.Select(city => new
            {
                city.Id,
                cityNumber = city.CityNumber,
                city.Name,
                projects = city.Projects.Select(project => new
                {
                    project.Id,
                    project.Name,
                    projectNumber = project.ProjectNumber,
                    cityId = project.CityId,
                    farms = project.Farms.Select(farm => new
                    {
                        farm.Id,
                        farm.FarmNumber,
                        farm.OwnerId,
                        farm.ProjectId,
                        boundaries = farm.Boundaries,  // سيتعبأ الآن
                        farm.Area,
                        documents = farm.Documents.Select(doc => new
                        {
                             doc.Id,
                            fileName = doc.FileName
                        }).ToList()
                    }).ToList()
                }).ToList()
            }).ToList();

            return Ok(tree);
        }



    }
}
