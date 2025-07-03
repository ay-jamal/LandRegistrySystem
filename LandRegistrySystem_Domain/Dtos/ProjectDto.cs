using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }

        public string Name { get; set; }           // اسم المشروع
        public DateTime CreatedDate { get; set; }  // تاريخ إنشاء المشروع
        public int FarmsCount { get; set; }     // عدد المزارع
        public int OwnersCount { get; set; }    // عدد الملاك المختلفين
        public int CityId {  get; set; }
        public string CityName { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
