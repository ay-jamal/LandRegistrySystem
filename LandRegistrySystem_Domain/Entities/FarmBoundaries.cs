using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Entities
{
    public class FarmBoundaries
    {
        public int Id { get; set; }
        public string North { get; set; }
        public string South { get; set; }
        public string East { get; set; }
        public string West { get; set; }

        public int FarmId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Farm Farm { get; set; }
    }
}
