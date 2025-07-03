using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.IRepositories
{
    public interface IFileManagerRepository
    {
        string CreateCityFolder(string cityNumber);
        string CreateFarmFolder(string cityNumber, string projectNumber, string farmNumber);
        string CreateProjectFolder(string cityNumber,string folderName); // ✅ أضف هذه
        Task<string> SaveFileAsync(string directory, IFormFile file);
        Task<byte[]> GetFileAsync(string path);
        bool DeleteFile(string path);
        Task<List<string>> GetFilesInDirectory(string folderPath);

    }
}
