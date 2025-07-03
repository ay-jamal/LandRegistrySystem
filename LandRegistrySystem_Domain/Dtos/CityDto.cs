using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Dtos
{
    public class CityDto
    {
        public int Id { get; set; }
        public string CityNumber { get; set; }
        public string Name { get; set; }
        public int ProjectsCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UpdatedByUser { get; set; }

        
    }
}
