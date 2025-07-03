using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Requests
{
    public class CreateFarmRequest
    {

        [Required]
        public string FarmNumber { get; set; }

        public double Area { get; set; }

        public int ProjectId { get; set; }  

        public int OwnerId { get; set; }

        // Boundaries
        public string North { get; set; }
        public string South { get; set; }
        public string East { get; set; }
        public string West { get; set; }
    }
}
