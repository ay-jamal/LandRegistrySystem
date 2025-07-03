using LandRegistrySystem_Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string Name { get; set; }           // اسم المشروع
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // العلاقة: المشروع يحتوي على عدة مزارع
        public List<Farm> Farms { get; set; } = new();
        public int CityId { get; set; }
        public City City { get; set; }
        public void Update(ProjectUpdateRequest project)
        {
            Name = project.Name;
            ProjectNumber = project.ProjectNumber;
            CityId = project.CityId;
        }

    } 
}
