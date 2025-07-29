using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Dtos
{
    public class FarmDto
    {
        public int Id { get; set; }
        public string FarmNumber { get; set; }
        public string CityNumber { get; set; }
        public string CityName { get; set; }
        public int CityId { get; set; }
        public string ProjectName { get; set; }

        public string ProjectNumber { get; set; }
        public int ProjectId { get; set; }

        public double Area { get; set; }
        public BoundaryDto Boundaries { get; set; }
        public OwnerDto Owner { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string UpdatedByUser { get; set; }

    }
}
