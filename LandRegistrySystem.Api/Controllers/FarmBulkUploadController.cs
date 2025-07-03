using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class FarmBulkUploadController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IFileManagerRepository _fileManager;
        private readonly int _defaultUserId = 1;
        private readonly string _rootPath;


        public FarmBulkUploadController(AppDbContext dbContext, IFileManagerRepository fileManager, IConfiguration configuration
)
        {
            _dbContext = dbContext;
            _fileManager = fileManager;
            _rootPath = configuration["FileStorage:RootPath"] ?? "C:\\LandFiles\\";
        }

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultipleFarms([FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("لا توجد ملفات للرفع");

            var errors = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    var fileName = file.FileName;
                    if (fileName.Length < 8)
                    {
                        errors.Add($"اسم الملف {fileName} غير صالح لتحديد المدينة والمشروع والمزرعة");
                        continue;
                    }

                    var code = fileName.Substring(0, 8); // عدل حسب طول الكود

                    var cityNumber = code.Substring(0, 2);
                    var projectNumber = code.Substring(2, 2);
                    var farmNumber = code.Substring(4   , 4);

                    var city = await _dbContext.Citites.FirstOrDefaultAsync(c => c.CityNumber == cityNumber);
                    if (city == null)
                    {
                        errors.Add($"المدينة برقم {cityNumber} غير موجودة (ملف {fileName})");
                        continue;
                    }

                    var project = await _dbContext.Projects
                        .FirstOrDefaultAsync(p => p.CityId == city.Id && p.ProjectNumber == projectNumber);
                    if (project == null)
                    {
                        errors.Add($"المشروع برقم {projectNumber} غير موجود في المدينة {cityNumber} (ملف {fileName})");
                        continue;
                    }

                    var farm = await _dbContext.Farms
                        .FirstOrDefaultAsync(f => f.ProjectId == project.Id && f.FarmNumber == farmNumber);

                    if (farm == null)
                    {
                        farm = new Farm
                        {
                            ProjectId = project.Id,
                            FarmNumber = farmNumber,
                            OwnerId = _defaultUserId,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _dbContext.Farms.AddAsync(farm);
                        await _dbContext.SaveChangesAsync();
                    }

                    _fileManager.CreateProjectFolder(cityNumber, projectNumber);
                    var farmFolderPath = _fileManager.CreateFarmFolder(cityNumber, projectNumber, farmNumber);

                    var savedFilePath = await _fileManager.SaveFileAsync(farmFolderPath, file);

                    var document = new FarmDocument
                    {
                        FarmId = farm.Id,
                        FileName = file.FileName,
                        FilePath = savedFilePath,
                        UploadedAt = DateTime.UtcNow
                    };
                    await _dbContext.FarmDocuments.AddAsync(document);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    errors.Add($"خطأ في رفع الملف {file.FileName}: {ex.Message}");
                    // لا نكسر اللوب، نكمل على باقي الملفات
                }
            }

            if (errors.Any())
                return BadRequest(new { Message = "تم رفع بعض الملفات مع وجود أخطاء", Errors = errors });

            return Ok("تم رفع الملفات وربطها بالمزارع بنجاح");
        }

        [HttpPost("import-from-server-folder")]
        public async Task<IActionResult> ImportFarmsFromServerFolder([FromBody] string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return BadRequest("Folder path must be provided.");

            if (!Directory.Exists(folderPath))
                return NotFound("Folder path does not exist on server.");

            var result = new List<string>();
            try
            {
                var farmFolders = Directory.GetDirectories(folderPath);

                foreach (var farmFolder in farmFolders)
                {
                    try
                    {
                        string farmFolderName = Path.GetFileName(farmFolder);

                        string cityNumber = farmFolderName.Substring(0, 2);
                        string projectNumber = farmFolderName.Substring(2, 2);
                        string farmNumber = farmFolderName.Substring(4, 4);

                        var city = await _dbContext.Citites.FirstOrDefaultAsync(c => c.CityNumber == cityNumber);
                        if (city == null)
                        {
                            city = new City { CityNumber = cityNumber, Name = "City " + cityNumber };
                            _dbContext.Citites.Add(city);
                            await _dbContext.SaveChangesAsync();
                        }

                        var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectNumber == projectNumber && p.CityId == city.Id);
                        if (project == null)
                        {
                            project = new Project { ProjectNumber = projectNumber, CityId = city.Id, Name = "Project " + projectNumber };
                            _dbContext.Projects.Add(project);
                            await _dbContext.SaveChangesAsync();
                        }

                        // ترتيب الملفات حسب الأقدم أولاً (من اسم الملف الذي يحوي التوقيت)
                        var files = Directory.GetFiles(farmFolder, "*.pdf")
                                             .OrderBy(f =>
                                             {
                                                 // توقع أن اسم الملف فيه التاريخ بالصيغة التي ذكرتها
                                                 // مثال: Doc0709043527_06_202516162271704.pdf
                                                 var name = Path.GetFileNameWithoutExtension(f);
                                                 var parts = name.Split('_');
                                                 if (parts.Length > 1 && long.TryParse(parts.Last(), out long timestamp))
                                                     return timestamp;
                                                 return 0L;
                                             }).ToList();

                        // قائمة الأحرف العربية لتسلسل التسمية
                        var arabicLetters = new[] { "", "أ", "ب", "ج", "د", "هـ", "و", "ز", "ح", "ط", "ي", "ك", "ل", "م", "ن", "س", "ع", "ف", "ص", "ق", "ر", "ش", "ت", "ث", "خ", "ذ", "ض", "ظ", "غ" };

                        for (int i = 0; i < files.Count; i++)
                        {
                            var file = files[i];
                            var fileName = Path.GetFileName(file);

                            // رقم المزرعة: الأصلي للأول، والبقية مع الحرف
                            string farmNumberThis = farmNumber + (i == 0 ? "" : arabicLetters.Length > i ? arabicLetters[i] : ("_" + i));

                            // تحقق ألا يكون هناك تكرار
                            var existingFarm = await _dbContext.Farms.FirstOrDefaultAsync(f => f.FarmNumber == farmNumberThis && f.ProjectId == project.Id);
                            if (existingFarm != null)
                            {
                                result.Add($"Farm {farmNumberThis} already exists. Skipped.");
                                continue;
                            }

                            // إنشاء المزرعة
                            var farm = new Farm
                            {
                                FarmNumber = farmNumberThis,
                                ProjectId = project.Id,
                                OwnerId = _defaultUserId // تأكد أن هذا المستخدم موجود في جدول Owners
                            };
                            _dbContext.Farms.Add(farm);
                            await _dbContext.SaveChangesAsync();

                            // نسخ الملف
                            var destinationPath = Path.Combine(_rootPath, cityNumber, cityNumber + projectNumber, cityNumber + projectNumber + farmNumberThis, fileName);
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                            System.IO.File.Copy(file, destinationPath, overwrite: true);

                            // حفظ المستند
                            var farmDocument = new FarmDocument
                            {
                                FarmId = farm.Id,
                                FileName = fileName,
                                FilePath = destinationPath,
                                UploadedAt = DateTime.Now
                            };
                            _dbContext.FarmDocuments.Add(farmDocument);

                            await _dbContext.SaveChangesAsync();
                            result.Add($"Farm {farmNumberThis} (file {fileName}) added successfully.");
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = ex.Message;
                        if (ex.InnerException != null)
                        {
                            errorMessage += " | Inner Exception: " + ex.InnerException.Message;
                            Exception inner = ex.InnerException;
                            while (inner.InnerException != null)
                            {
                                inner = inner.InnerException;
                                errorMessage += " -> " + inner.Message;
                            }
                        }
                        result.Add($"Error processing folder {farmFolder}: {errorMessage}");
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | Inner Exception: " + ex.InnerException.Message;
                    Exception inner = ex.InnerException;
                    while (inner.InnerException != null)
                    {
                        inner = inner.InnerException;
                        errorMessage += " -> " + inner.Message;
                    }
                }
                return StatusCode(500, $"Server error: {errorMessage}");
            }
        }


    }
}
