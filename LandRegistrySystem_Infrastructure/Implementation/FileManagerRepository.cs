using LandRegistrySystem_Domain.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Infrastructure.Implementation
{
    public class FileManagerRepository : IFileManagerRepository
    {
        private readonly string _rootPath;

        public FileManagerRepository(IConfiguration configuration)
        {
            _rootPath = configuration["FileStorage:RootPath"];
        }

        public string CreateCityFolder(string cityNumber)
        {
            var cityPath = Path.Combine(_rootPath, cityNumber);
            if (!Directory.Exists(cityPath))
                Directory.CreateDirectory(cityPath);

            return cityPath;
        }

        public string CreateProjectFolder(string cityNumber, string projectNumber)
        {

            var cityPath = Path.Combine(_rootPath, cityNumber);
            if (!Directory.Exists(cityPath))
                Directory.CreateDirectory(cityPath);

            // مجلد المشروع داخل مجلد المدينة، اسمه رقم المدينة + رقم المشروع مع بعض
            var projectFolderName = cityNumber + projectNumber;
            var projectPath = Path.Combine(cityPath, projectFolderName);
            if (!Directory.Exists(projectPath))
                Directory.CreateDirectory(projectPath);

            return projectPath;
        }

        public string CreateFarmFolder(string cityNumber, string projectNumber, string farmNumber)
        {
            // مجلد المدينة
            var cityPath = Path.Combine(_rootPath, cityNumber);
            if (!Directory.Exists(cityPath))
                Directory.CreateDirectory(cityPath);

            // مجلد المشروع داخل مجلد المدينة (اسم المجلد = رقم المدينة + رقم المشروع)
            var projectFolderName = cityNumber + projectNumber;
            var projectPath = Path.Combine(cityPath, projectFolderName);
            if (!Directory.Exists(projectPath))
                Directory.CreateDirectory(projectPath);

            // مجلد المزرعة داخل مجلد المشروع (اسم المجلد = رقم المدينة + رقم المشروع + رقم المزرعة)
            var farmFolderName = cityNumber + projectNumber + farmNumber;
            var farmPath = Path.Combine(projectPath, farmFolderName);
            if (!Directory.Exists(farmPath))
                Directory.CreateDirectory(farmPath);

            return farmPath;
        }

        public async Task<string> SaveFileAsync(string directory, IFormFile file)
        {
            var filePath = Path.Combine(directory, file.FileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return filePath;
        }

        public async Task<List<string>> GetFilesInDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return new List<string>();

            return await Task.Run(() => Directory.GetFiles(folderPath).ToList());
        }

        public async Task<byte[]> GetFileAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }

        public bool DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }
    }
}
