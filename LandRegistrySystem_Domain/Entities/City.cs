using LandRegistrySystem_Domain.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.Entities
{
    public class City
    {
        public int Id { get; set; }

        public string CityNumber { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<Project> Projects { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }  // nullable لأن ممكن ما يتحدثش من البداية
        public string UpdatedByUser { get; set; }
        public void Update(UpdateCityRequest city,string updatedByUser)
        {
            CityNumber = city.CityNumber;
            Name = city.Name;
            UpdatedAt = DateTime.Now;
            UpdatedByUser = updatedByUser;
        }

    }
}
